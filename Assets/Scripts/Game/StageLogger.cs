using StageInfo;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

public class StageLogger : MonoBehaviour
{
    void Start()
    {
        StageData s = Stage1.Stage1_1;
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("========== Stage1_1 ==========");
        sb.AppendLine($"ステージ横: {s.STAGE_WIDTH}");
        sb.AppendLine($"ステージ縦: {s.STAGE_HEIGHT}");
        sb.AppendLine("");
        sb.AppendLine($"表スタート位置: [{s.START_POS_front.X}, {s.START_POS_front.Y}]");
        sb.AppendLine($"表ゴール位置: [{s.GOAL_POS_front.X}, {s.GOAL_POS_front.Y}]");
        sb.AppendLine($"裏スタート位置: [{s.START_POS_back.X}, {s.START_POS_back.Y}]");
        sb.AppendLine($"裏ゴール位置: [{s.GOAL_POS_back.X}, {s.GOAL_POS_back.Y}]");
        sb.AppendLine("");
        sb.AppendLine($"魂の位置: {s.SOUL_POS.IsLeft}, 魂の座標: [{s.SOUL_POS.X}, {s.SOUL_POS.Y}]");
        sb.AppendLine("");
        sb.AppendLine($"向き変更可能か: {s.isDIRECTION}, 王様は左向きか: {s.isKING_LEFT}, 姫様は左向きか: {s.isQUEEN_LEFT}");
        sb.AppendLine("");
        sb.AppendLine("縫える回数 と 縫える長さ: " + string.Join(", ", s.STRING_COUNT));

        // シグニチャ
        void AppendObjs(string name, IReadOnlyList<Int2> arr)
        {
            sb.AppendLine($"{name} ({arr.Count}):");
            if (arr.Count == 0)
                sb.AppendLine("(none)");
            else
                foreach (var p in arr)
                    sb.AppendLine($"[{p.X}, {p.Y}]");

            sb.AppendLine("");
        }

        void AppendWinds(string name, IReadOnlyList<WindPos> arr)
        {
            sb.AppendLine($"{name} ({arr.Count}):");
            if (arr.Count == 0)
                sb.AppendLine("(none)");
            else
                foreach (var w in arr)
                    sb.AppendLine($"向き: [{w.Dir}], [{w.X}, {w.Y}]");

            sb.AppendLine("");
        }

        AppendObjs("表: 鉄板 の位置: ", s.STEEL_front);
        AppendObjs("裏: 鉄板 の位置: ", s.STEEL_back);
        AppendObjs("表: しわ の位置: ", s.WRINKLE_front);
        AppendObjs("表: しわ の位置: ", s.WRINKLE_back);
        AppendWinds("表: 風穴 の位置: ", s.WIND_front);
        AppendWinds("表: 風穴 の位置: ", s.WIND_back);

        Debug.Log(sb.ToString());
    }
}
