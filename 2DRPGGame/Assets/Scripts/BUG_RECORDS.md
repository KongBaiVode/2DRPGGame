# Bug 记录文档

> 2D RPG Unity 游戏项目 — Bug 追踪与修复记录

---

## Bug #1: 玩家攻击方向偶尔反向

**状态**：✅ 已修复  
**发现日期**：2026-07  
**严重程度**：中（影响战斗体验，但并不总是发生）

---

### 问题描述

玩家执行以下操作时，角色**偶尔**会朝错误的方向攻击：

1. 按 **A 键**向左移动一小段距离
2. 按 **D 键**向右移动一小段距离
3. 松开 D 键
4. 按**鼠标左键**进行攻击

**预期行为**：角色朝右攻击（因为最后移动方向是右，`facingDir = 1`）

**实际行为**：角色**偶尔朝左攻击**

**调试发现**：在 `PlayerPrimaryAttackState.Enter()` 中，`player.facingDir == 1`（正确），但 `xInput == -1`（错误）。

---

### 根因分析

#### 核心原因：`PrimaryAttackState` 实例的 `xInput` 字段跨攻击调用残留

`xInput` 是 `PlayerState` 基类中定义的 `protected` 实例字段（`PlayerState.cs:13`）：

```csharp
protected float xInput;
```

每个状态（`MoveState`、`IdleState`、`PrimaryAttackState` 等）都是独立的 C# 对象实例，各自拥有独立的 `xInput` 字段。`xInput` 只在各自状态的 `Update()` → `PlayerState.Update()` 中被更新（`PlayerState.cs:46`）：

```csharp
public virtual void Update()
{
    // ...
    xInput = Input.GetAxisRaw("Horizontal");
    // ...
}
```

#### 脏数据产生过程

```
上一次攻击的某一帧:
  PrimaryAttack.Update()
    └→ PlayerState.Update()
        └→ PrimaryAttackState.xInput = Input.GetAxisRaw("Horizontal") = -1  ← 玩家此时按了A键

攻击结束，状态切换:
  ChangeState(IdleState)
    └→ PrimaryAttack.Exit()  ← xInput 仍然是 -1，无人清理
    └→ IdleState.Enter()

后续移动阶段:
  IdleState.Update()   → IdleState.xInput       = 0  (每帧更新，正确)
  MoveState.Update()   → MoveState.xInput        = 1  (每帧更新，正确)
  PrimaryAttackState.xInput                       = -1 ← 冻结！无人更新！

新攻击触发:
  ChangeState(PrimaryAttack)
    └→ PrimaryAttack.Enter()
        └→ 读取 this.xInput = -1  ← 脏数据！导致攻击方向错误！
```

#### 关键代码路径（`PlayerPrimaryAttackState.cs:35-41`）

```csharp
float attackDir = player.facingDir;   // 默认使用角色朝向

if(xInput != 0)                        // xInput 非零就覆盖
{
    attackDir = xInput;                // attackDir = -1 → 朝左攻击！
}
```

因为脏数据 `xInput == -1`（非零），`attackDir` 被覆盖为 -1，导致角色朝左攻击。而此时 `facingDir == 1`（MoveState 中通过 `SetVelocity` 正确设置的朝向），两者不一致。

#### 为什么"偶尔"发生

- 脏数据的值取决于**上一次攻击期间**玩家是否按过方向键
- 如果上一次攻击中玩家一直没按方向键，`xInput` 残留为 0，bug 不会触发
- 如果上一次攻击中玩家按过 A 键，`xInput` 残留为 -1，bug 触发
- 由于每场战斗中攻击发生的时机和玩家的按键情况不同，表现为"偶尔"

---

### 修复方案

**文件**：`Player\PlayerStates\PlayerPrimaryAttackState.cs`

**改动**：在 `Enter()` 方法的 `base.Enter()` 之后、攻击方向逻辑之前，增加一行：

```csharp
xInput = Input.GetAxisRaw("Horizontal");
```

**修复后的 `Enter()` 方法**：

```csharp
public override void Enter()
{
    base.Enter();

    // Fix Bug#1: 重新读取当前帧的输入，避免使用上一次攻击残留的脏数据
    xInput = Input.GetAxisRaw("Horizontal");

    if(comboCounter > 2 || Time.time > lastTimeAttacked + comboWindow)
        comboCounter = 0;

    player.animator.SetInteger("ComboCounter", comboCounter);

    player.attackNum = comboCounter;

    #region  选择攻击方向
    float attackDir = player.facingDir;

    if(xInput != 0)
    {
        attackDir = xInput;
    }

    #endregion

    player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);

    stateTimer = 0.1f;
}
```

#### 为什么这个方案更好

项目中之前有一行被注释掉的修复尝试（`PlayerPrimaryAttackState.cs:23`）：

```csharp
//xInput = 0;  //修复Bug：玩家攻击方向偶尔反向的问题，本代码第40行。
//               但是这样改，玩家在攻击过程中就无法改变方向了。
```

| 方案 | `xInput = 0`（旧） | `xInput = Input.GetAxisRaw("Horizontal")`（新） |
|------|-------------------|----------------------------------------------|
| 是否修复脏数据 | ✅ 是 | ✅ 是 |
| 攻击中能否改变方向 | ❌ 不能（强制归零） | ✅ 能（读取真实输入） |

新方案**既修复了 bug，又保留了设计意图**：允许玩家在攻击瞬间通过按住方向键来改变攻击方向。

---

### 关联知识

- `xInput` 是**实例字段**而非静态字段，每个状态对象有独立的副本
- `ChangeState()` 在 `PlayerStateMachine` 中**同步执行** `Exit()` 和 `Enter()`（`PlayerStateMachine.cs:14-18`）
- `Input.GetAxisRaw("Horizontal")` 在 Unity 每帧内返回相同值，可以安全地在同帧多次调用
- `FlipController()` 在 `Entity.SetVelocity()` 中被调用（`Entity.cs:92`），会根据速度方向自动更新 `facingDir`
