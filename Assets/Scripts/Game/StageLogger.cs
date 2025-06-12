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
        sb.AppendLine($"�X�e�[�W��: {s.STAGE_WIDTH}");
        sb.AppendLine($"�X�e�[�W�c: {s.STAGE_HEIGHT}");
        sb.AppendLine("");
        sb.AppendLine($"�\�X�^�[�g�ʒu: [{s.START_POS_front.X}, {s.START_POS_front.Y}]");
        sb.AppendLine($"�\�S�[���ʒu: [{s.GOAL_POS_front.X}, {s.GOAL_POS_front.Y}]");
        sb.AppendLine($"���X�^�[�g�ʒu: [{s.START_POS_back.X}, {s.START_POS_back.Y}]");
        sb.AppendLine($"���S�[���ʒu: [{s.GOAL_POS_back.X}, {s.GOAL_POS_back.Y}]");
        sb.AppendLine("");
        sb.AppendLine($"���̈ʒu: {s.SOUL_POS.IsLeft}, ���̍��W: [{s.SOUL_POS.X}, {s.SOUL_POS.Y}]");
        sb.AppendLine("");
        sb.AppendLine($"�����ύX�\��: {s.isDIRECTION}, ���l�͍�������: {s.isKING_LEFT}, �P�l�͍�������: {s.isQUEEN_LEFT}");
        sb.AppendLine("");
        sb.AppendLine("�D����� �� �D���钷��: " + string.Join(", ", s.STRING_COUNT));

        // �V�O�j�`��
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
                    sb.AppendLine($"����: [{w.Dir}], [{w.X}, {w.Y}]");

            sb.AppendLine("");
        }

        AppendObjs("�\: �S�� �̈ʒu: ", s.STEEL_front);
        AppendObjs("��: �S�� �̈ʒu: ", s.STEEL_back);
        AppendObjs("�\: ���� �̈ʒu: ", s.WRINKLE_front);
        AppendObjs("�\: ���� �̈ʒu: ", s.WRINKLE_back);
        AppendWinds("�\: ���� �̈ʒu: ", s.WIND_front);
        AppendWinds("�\: ���� �̈ʒu: ", s.WIND_back);

        Debug.Log(sb.ToString());
    }
}
