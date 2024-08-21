using System.Collections;
using System.Collections.Generic;
using ShipInventory.Helpers;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShipInventory.Objects;

public class ChutePanel : NetworkBehaviour
{
    public static ChutePanel? Instance = null;
    public static int unlockIndex = -1;

    private void Start()
    {
        header = transform.Find("ChutePanelScreen").Find("Header").GetComponent<TextMeshProUGUI>();
        text = transform.Find("ChutePanelScreen").Find("Text").GetComponent<TextMeshProUGUI>();

        if (!NetworkManager.Singleton.IsHost)
            return;
        
        AddNews();
        StartCoroutine(SetNews());
    }

    private TextMeshProUGUI? header;
    private TextMeshProUGUI? text;

    public void SetText(string value) => text?.SetText(value + '\r');
    public void SetHeader(string value) => header?.SetText(value + '\r');

    #region Status

    private bool isIdling;
    private readonly List<(string header, string text)> news = [
        ("THE COMPANY IS", "WATCHING"),
        ("<align=center>THE MESS", "GONE"),
        ("I can spawn a\ndog, you know?", ""),
        ("<align=center><color=#0FF>\nYIPEE!", ""),
        ("Not even close", "   T^T"),
        ("<color=purple><align=center>\n*dances*", ""),
        ("<color=grey><align=center>\n*mine beep*", ""),
        ("<color=#AD0000>Road works ahead?<color=#460>\nI sure hope it\ndoes!", ""),
        ("<align=center><color=#990>\nBEES!?!!?!?", ""),
        ("<color=#0AA><align=center>\n*bonk*", ""),
        ("<color=#EEE>Sigurd...<color=#999>\nStop...<color=#444>\nPlease...", ""),
        ("<align=center>Gratar\nmy Love\n<color=#FAA><3", ""),
        ("<color=#074><align=center>beep boop bah\nbap bup beop\nbip beup bap", ""),
        ("<align=center>IT WASN'T\nTHE MIMIC????", ""),
        ("<color=#1BE>It's cheating\nunless I do it", ""),
        ("<color=yellow><align=center>The grind\nis real!", ""),
        ("<color=yellow>Chipi chipi\n<color=purple>Chapa Chapa", ""),
        ("Help I'm scared", "pwease?"),
        ("<align=center><color=#86cecb>Pipebomb!", "<color=#A0A>* BOOM *"),
        ("Wanna see <color=yellow>these</color>?", "<color=#D60>DEEZ NUTS")
    ];

    public void SetIdle()
    {
        SetServerRpc("<u>CURRENT STATUS</u>", "<color=green>IDLE</color>", true);
    }
    
    public void SetBlocked()
    {
        SetServerRpc("<u>CURRENT STATUS</u>", "<color=#870000>BLOCKED</color>");

        isIdling = false;
    }
    
    public void SetSpawning(int remaining)
    {
        SetServerRpc("<color=yellow><u>SPAWNING QUEUE</u></color>", $"{remaining} REMAINING");
        
        isIdling = false;
    }

    public void ShowTotal()
    {
        SetServerRpc("<u>TOTAL</u>", $"${ItemManager.GetTotalValue()}", true);
    }

    public void ShowAmount()
    {
        SetServerRpc("<u>COUNT</u>", $"<color=purple>{ItemManager.GetTotalValue()}</color>", true);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SetServerRpc(string _header, string _text, bool isIdle = false)
    {
        SetClientRpc(_header, _text, isIdle);
    }

    [ClientRpc]
    public void SetClientRpc(string _header, string _text, bool isIdle)
    {
        SetHeader(_header);
        SetText(_text);
        isIdling = isIdle;
    }

    #endregion

    #region News

    private readonly List<System.Action> cycledIdles = [];
    private int cycledIndex;

    private void AddNews()
    {
        cycledIdles.Add(SetIdle);
        cycledIdles.Add(ShowTotal);
        cycledIdles.Add(ShowAmount);
    }

    private IEnumerator SetNews()
    {
        while (news.Count > 0)
        {
            cycledIdles[cycledIndex]?.Invoke();

            cycledIndex++;

            if (cycledIndex >= news.Count)
                cycledIndex = 0;

            // Show if necessary
            if (ShipInventory.Config.ShowNews.Value)
            {
                yield return new WaitForSeconds(Random.Range(30f, 300f));
            
                if (isIdling)
                {
                    int index = Random.Range(0, news.Count);

                    SetServerRpc(news[index].header, news[index].text);
                }
            }
            
            yield return new WaitForSeconds(Random.Range(5f, 15f));
        }
        
        yield return new WaitForEndOfFrame();
    }

    #endregion
}