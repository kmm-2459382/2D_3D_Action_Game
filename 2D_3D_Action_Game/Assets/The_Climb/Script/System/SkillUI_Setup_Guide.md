# スキルUI実装ガイド

## 概要

スキルのクールタイムと使用状態を表示するUIシステムを実装しました。このドキュメントでは、Unityエディタでの設定方法を説明します。

## 実装済みのスクリプト

### 1. SkillUIManager.cs
- **場所**: `Assets/Script/System/SkillUIManager.cs`
- **機能**: スキルアイコンの表示とクールタイムゲージの管理

### 2. 拡張されたスクリプト
- **PlayerAbilityManager.cs**: 現在のスキル番号を公開するプロパティを追加
- **GravityFlipManager.cs**: クールタイム進行状況を取得するメソッドを追加
- **SkilTransparent.cs**: クールタイム管理機能を追加

## Unityエディタでの設定手順

### ステップ1: UIキャンバスの作成

1. **Hierarchy**で右クリック → **UI** → **Canvas**を選択
2. Canvasの名前を`SkillUICanvas`に変更
3. Canvas Scalerの設定:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920x1080

### ステップ2: スキルUIパネルの作成

1. `SkillUICanvas`を右クリック → **UI** → **Panel**を選択
2. Panelの名前を`SkillUIPanel`に変更
3. Rect Transformの設定(画面右下に配置):
   - Anchor Presets: 右下(Bottom Right)
   - Pos X: -150
   - Pos Y: 100
   - Width: 250
   - Height: 150

### ステップ3: スキルアイコン画像の作成

1. `SkillUIPanel`を右クリック → **UI** → **Image**を選択
2. Imageの名前を`SkillIconImage`に変更
3. Rect Transformの設定:
   - Anchor Presets: 中央
   - Pos X: 0
   - Pos Y: 40
   - Width: 80
   - Height: 80

### ステップ4: クールタイムゲージの作成

各スキル用に3つのゲージを作成します:

#### Umedaクールタイムゲージ
1. `SkillUIPanel`を右クリック → **UI** → **Image**を選択
2. Imageの名前を`UmedaCooldownGauge`に変更
3. Rect Transformの設定:
   - Anchor Presets: 左
   - Pos X: -80
   - Pos Y: -20
   - Width: 60
   - Height: 80
4. Imageコンポーネントの設定:
   - Image Type: Filled
   - Fill Method: Vertical
   - Fill Origin: Top
   - Color: 好みの色(例: 青系)

#### Kitanoクールタイムゲージ
1. `SkillUIPanel`を右クリック → **UI** → **Image**を選択
2. Imageの名前を`KitanoCooldownGauge`に変更
3. Rect Transformの設定:
   - Anchor Presets: 中央
   - Pos X: 0
   - Pos Y: -20
   - Width: 60
   - Height: 80
4. Imageコンポーネントの設定:
   - Image Type: Filled
   - Fill Method: Vertical
   - Fill Origin: Top
   - Color: 好みの色(例: 緑系)

#### Nisiyamaクールタイムゲージ
1. `SkillUIPanel`を右クリック → **UI** → **Image**を選択
2. Imageの名前を`NisiyamaCooldownGauge`に変更
3. Rect Transformの設定:
   - Anchor Presets: 右
   - Pos X: 80
   - Pos Y: -20
   - Width: 60
   - Height: 80
4. Imageコンポーネントの設定:
   - Image Type: Filled
   - Fill Method: Vertical
   - Fill Origin: Top
   - Color: 好みの色(例: 赤系)

### ステップ5: SkillUIManagerコンポーネントの追加

1. `SkillUIPanel`を選択
2. **Inspector**で**Add Component**をクリック
3. `SkillUIManager`を検索して追加
4. 以下のフィールドを設定:

#### スキル状態画像
- **Skill Icon Image**: `SkillIconImage`をドラッグ&ドロップ
- **Umeda Skill Sprite**: `Assets/Img/Image/Ability/IMG_3054.PNG`をドラッグ&ドロップ
- **Kitano Skill Sprite**: `Assets/Img/Image/Ability/IMG_3056.PNG`をドラッグ&ドロップ
- **Nisiyama Skill Sprite**: `Assets/Img/Image/Ability/IMG_3055.PNG`をドラッグ&ドロップ

#### クールタイムゲージ
- **Umeda Cooldown Gauge**: `UmedaCooldownGauge`をドラッグ&ドロップ
- **Kitano Cooldown Gauge**: `KitanoCooldownGauge`をドラッグ&ドロップ
- **Nisiyama Cooldown Gauge**: `NisiyamaCooldownGauge`をドラッグ&ドロップ

#### 参照
- **Ability Manager**: Playerオブジェクトの`PlayerAbilityManager`をドラッグ&ドロップ
- **Skill Platform Spawner**: Playerオブジェクトの`SkillPlatformSpawner`をドラッグ&ドロップ
- **Gravity Flip Manager**: シーン内の`GravityFlipManager`をドラッグ&ドロップ
- **Skil Transparent**: Playerオブジェクトの`SkilTransparent`をドラッグ&ドロップ

### ステップ6: 動作確認

1. Unityエディタで再生ボタンを押す
2. `LeftShift`キーでスキルを切り替え
3. スキルアイコンが切り替わることを確認
4. 各スキルを使用してクールタイムゲージが上から下に減少することを確認

## 注意事項

- **画像アセット**: 3つの画像(`IMG_3054.PNG`、`IMG_3055.PNG`、`IMG_3056.PNG`)がスキルアイコンとして使用されます。必要に応じて、Unityエディタで画像のImport Settingsを調整してください(Texture Type: Sprite (2D and UI))。

## カスタマイズ

### クールタイムの調整
- **Umeda**: `SkillPlatformSpawner.cs`の`skillCooldown`フィールド(デフォルト: 3秒)
- **Kitano**: `SkilTransparent.cs`の`cooldownTime`フィールド(デフォルト: 3秒)
- **Nisiyama**: `GravityFlipManager.cs`の`flipCooldown`フィールド(デフォルト: 10秒)

### UI配置の変更
`SkillUIPanel`のRect Transformを調整することで、UI全体の位置を変更できます。

### ゲージの色変更
各クールタイムゲージのImageコンポーネントのColorフィールドで色を変更できます。
