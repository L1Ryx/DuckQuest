using System.Text;
using TMPro;
using UnityEngine;

public class InventoryDebugOverlay : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float refreshRateHz = 10f;

    private float timer;

    private void Reset()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (!Game.IsReady || Game.Ctx.Inventory == null || text == null)
            return;

        timer += Time.deltaTime;
        float interval = 1f / Mathf.Max(1f, refreshRateHz);
        if (timer < interval) return;
        timer = 0f;

        var inv = Game.Ctx.Inventory;
        var db = Game.Ctx.ItemDb;

        var sb = new StringBuilder(256);
        sb.AppendLine("Inventory:");

        if (inv.Data.entries.Count == 0)
        {
            sb.AppendLine("  (empty)");
        }
        else
        {
            var sel = Game.Ctx.InventorySelection;
            sel?.EnsureValid();
            int selectedIndex = sel != null ? sel.SelectedIndex : -1;

            for (int i = 0; i < inv.Data.entries.Count; i++)
            {
                var e = inv.Data.entries[i];

                bool isSelected = (i == selectedIndex);
                sb.Append(isSelected ? "> " : "  ");

                var def = db != null ? db.Get(e.itemId) : null;
                string name = def != null && !string.IsNullOrEmpty(def.displayName) ? def.displayName : e.itemId;

                int packSize = 0;
                if (def is HardwormPackDefinition wormDef)
                    packSize = wormDef.packSize;

                sb.Append("• ")
                    .Append(name)
                    .Append("  x")
                    .Append(e.count);

                if (packSize > 0)
                    sb.Append("  (pack=").Append(packSize).Append(')');

                sb.AppendLine();
            }
        }


        text.text = sb.ToString();
    }
}