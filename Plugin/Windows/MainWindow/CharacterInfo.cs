using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using Plugin.Internal;

namespace Plugin.Windows.MainWindow;

internal class CharacterInfo
{
    internal static void DrawCharacterInfo()
    {

        if (ImGui.BeginTabBar("##CharacterInfoBar"))
        {
            if (ImGui.BeginTabItem("Basic"))
            {
                DrawBasicInfo();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Retainers"))
            {
                DrawRetainers();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }

    internal static void DrawBasicInfo()
    {
        ImGui.Text("Character Details");
        ImGui.Text("name: " + Player.Name);
        ImGui.Text("Level: " + Player.Level);
        ImGui.Text("Job: " + Player.Job.ToString());

    }

    internal static unsafe void DrawRetainers()
    {
        if (!GameRetainerManager.IsReady)
        {
            ImGui.Text("Not ready yet!");
            Svc.Log.Warning("not ready oops");
            return;
        }

        var retainerManager = RetainerManager.Instance();

        var retainers = GameRetainerManager.GetRetainers();

        // Create a dictionary to map retainers to their display order
        var displayOrderMap = new Dictionary<ulong, int>();

        // Use a fixed block to pin the span in memory
        fixed (byte* displayOrderPtr = retainerManager->DisplayOrder)
        {
            for (int i = 0; i < retainers.Length; i++)
            {
                var retainer = retainers[i];
                displayOrderMap[retainer.RetainerID] = displayOrderPtr[i];
            }
        }

        // Sort retainers by display order
        GameRetainerManager.Retainer[]? sortedRetainers = retainers
        .OrderBy(r => displayOrderMap.ContainsKey(r.RetainerID) ? displayOrderMap[r.RetainerID] : int.MaxValue)
        .ToArray();

        if (ImGui.BeginTable("RetainersTable", 8, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            // Define columns
            ImGui.TableSetupColumn("Display Order Index");
            ImGui.TableSetupColumn("Internal Index");
            ImGui.TableSetupColumn("Retainer ID");
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Class/Job");
            ImGui.TableSetupColumn("Inventory");
            ImGui.TableSetupColumn("Gil");
            ImGui.TableSetupColumn("Items for Sale");

            ImGui.TableHeadersRow();

            for (int i = 0; i < sortedRetainers.Length; i++)
            {
                GameRetainerManager.Retainer? retainer = sortedRetainers[i];
                var displayOrderIndex = displayOrderMap.ContainsKey(retainer.RetainerID) ? displayOrderMap[retainer.RetainerID] : -1;

                ImGui.TableNextRow();

                // Display columns
                ImGui.TableSetColumnIndex(0);
                ImGui.Text(displayOrderIndex.ToString());

                ImGui.TableSetColumnIndex(1);
                ImGui.Text(i.ToString()); // Internal index


                ImGui.TableSetColumnIndex(2);
                ImGui.Text(retainer.RetainerID.ToString());

                ImGui.TableSetColumnIndex(3);
                ImGui.Text(retainer.Name);



                ImGui.TableSetColumnIndex(4);
                if (ThreadLoadImageHandler.TryGetIconTextureWrap(retainer.ClassJob == 0 ? 62143 : 062100 + retainer.ClassJob, true, out var t))
                {
                    ImGui.Image(t.ImGuiHandle, new(24, 24));
                }
                else
                {
                    ImGui.Dummy(new(24, 24));
                }
                ImGui.SameLine();
                string classJobName = ExcelJobHelper.GetJobNameById(retainer.ClassJob);
                ImGui.Text("" + retainer.Level.ToString());





                ImGui.TableSetColumnIndex(5);
                if (ThreadLoadImageHandler.TryGetIconTextureWrap(60356, true, out var inv))
                {
                    ImGui.Image(inv.ImGuiHandle, new(24, 24));
                }
                else
                {
                    ImGui.Dummy(new(24, 24));
                }
                ImGui.SameLine();
                ImGui.Text($"{retainer.ItemCount}/175");



                ImGui.TableSetColumnIndex(6);
                if (ThreadLoadImageHandler.TryGetIconTextureWrap(065002, true, out var g))
                {
                    ImGui.Image(g.ImGuiHandle, new(24, 24));
                }
                else
                {
                    ImGui.Dummy(new(24, 24));
                }
                ImGui.SameLine();
                ImGui.Text($"{retainer.Gil.ToString("N0")}");




                ImGui.TableSetColumnIndex(7);
                //if (ThreadLoadImageHandler.TryGetIconTextureWrap(060570, true, out var g))
                //{
                //    ImGui.Image(g.ImGuiHandle, new(24, 24));
                //}
                //else
                //{
                //    ImGui.Dummy(new(24, 24));
                //}

                if (ThreadLoadImageHandler.TryGetIconTextureWrap(GameRetainerManager.GetTownIconID((retainer.Town)), true, out var town))
                {
                    ImGui.Image(town.ImGuiHandle, new System.Numerics.Vector2(24, 24));
                }
                else
                {
                    ImGui.Dummy(new System.Numerics.Vector2(24, 24));
                }
                ImGui.SameLine();
                ImGui.Text($"Selling {retainer.MarketItemCount} items");


            }

            ImGui.EndTable();
        }
    }
}
