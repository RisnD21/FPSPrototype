最後更新於 2025/07/12
# Conversation System
希望能更加方便的實現角色與場景、物品的互動

## 功能
### 主要功能：
1. 在 Editor 環境中建立人物的線性對話劇本
2. 以任務為單位，每個任務可指定前中後的對話
3. 對話則透過選項，指定點擊選項後所對應的對話
 

### 額外功能：
1. 附加物品欄系統，包含基本的添加、移除、查找等功能
2. 簡單易用的 OnTrigger 介面、EventBus
3. 任務系統，可發布、完成並統計當前任務
4. 自動廣播系統訊息，提供介面供自定
5. 上述已整合並測試完畢，見於 SampleScene
### 限制：
1. 尚未實現依條件更改選項行為
2. 單一角色同時僅能提供一則任務
3. 無防呆，請自行避免任何重複ID
## 使用方式 （可參考 sample)
### 前置準備：
1. 導入模組後，建立 managers prefab 
2. 建立 Dialogue Container，亦可自訂 UI

### 建立對話：
1. 將 QuestNarrator 拖曳至觸發對話的物件
2. 指定 Interact() 為觸發方式
3. 透過 Create > GameJam > Conversation, Quest:
4. 定義 QuestChainEntry
5. 定義 前中後三則對話 Scripts
6. 設定內容、選項

### 建立任務：
1. 透過 Create > GameJam > Quest
2. 定義 QuestData
3. 透過 Create > GameJam > OnTrigger
4. 建立 OnTrigger: StartQuest 並指定剛剛的 QuestData
5. 建立 OnTrigger: TryComplete 並指定剛剛的 QuestData
6. 於選項 OnTrigger 欄位中指定所需觸發的項目
7. 若有需要 Unity Event 調用場景物件，則建立 Create > Gamejam > Quest > QuestEvent
8. 並在場景中的物件上加 Quest Event Listener 插件
9. 指定對應的監聽 Channel （剛剛的 QuestEvent)
### 定義物品
1. 於目錄 Quest Dialogue System > Resources 下 Items 資料夾中
2. 建立 Gamejam > Item