using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class PhotonPrompt : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public bool isConnect;
    [SerializeField] private Text statusText;
    private const string roomName = "ComfyUIServer";
    private PhotonView photonView;
    public texttoimage texttoimage;
    public MenuUI menuUI;
    private int currentClientId = -1;
    private string latestPngId;
    private string latestLatentId;
    private string latestConditioningId;
    private string latestVAEId;
    private int clientID;
    private float checkInterval = 2f; 
    private bool isChecking = false;
    [SerializeField]
    private Button joinButton;
    [HideInInspector]
    public string status;
    [SerializeField]
    private Image image;
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
            Debug.LogError("PhotonView component not found!");
    }

    void Start()
    {
        isConnect = false;
        status = "finish";
        joinButton.gameObject.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
        UpdateStatus("Connecting...");
        image.color = Color.gray;
    }

    // 建立房間 (Master Client用)
    public void CreateRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            RoomOptions options = new RoomOptions { MaxPlayers = 10 };
            PhotonNetwork.CreateRoom(roomName, options);
            UpdateStatus("Room Created");
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        bool foundRoom = false;
        foreach (RoomInfo room in roomList)
        {
            if (room.Name == roomName && !room.RemovedFromList)
            {
                foundRoom = true;
                break;
            }
        }

        if (foundRoom)
        {
            UpdateStatus("Server ready, click to join");
            if (joinButton != null)
                joinButton.gameObject.SetActive(true);
        }
        else
        {
            UpdateStatus("Waiting for server startup...");
            image.color = Color.gray;
            if (joinButton != null)
            {
                joinButton.gameObject.SetActive(false);
                isConnect = false;
            }                
        }
    }


    // 加入房間 (一般Client用)
    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }
    public void OnDisconnect()
    {
        PhotonNetwork.Disconnect();
    }
    [PunRPC]
    private void ShareClientId(int clientId)
    {
        Debug.Log($"Client ID received: {clientId}");
        // 這裡可以存儲其他客戶端的ID
        if (clientId != currentClientId)
        {
            Debug.Log($"Other client connected with ID: {clientId}");
        }
    }
    [PunRPC]
    // 發送 prompt
    public void SendPrompt(string User_Model, string User_width, string User_height, string User_seed, string User_steps, string User_cfg, string User_prompt)
    {
        if (!PhotonNetwork.InRoom) return;

        int senderId = PhotonNetwork.LocalPlayer.ActorNumber;

        if (PhotonNetwork.IsMasterClient)
        {
            UpdateStatus("你是主機，無法傳送訊息給自己");
            return;
        }
        photonView.RPC("ReceivePrompt", PhotonNetwork.MasterClient, User_Model, User_width, User_height, User_seed, User_steps, User_cfg, User_prompt, senderId);
    }

    [PunRPC]
    private void ReceivePrompt(string User_width, string User_height, string User_seed, string User_steps, string User_cfg, string User_prompt, int senderId)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        string allparameter = $"width: {User_width} height: {User_height} seed: {User_seed} steps: {User_steps} cfg: {User_cfg}";
        Debug.Log($"收到來自玩家 {senderId} 的參數: {allparameter}");
        UpdateStatus($"收到來自玩家 {senderId} 的參數: {allparameter}");

        // 設定參數
        texttoimage.SetAllParametertxt2img(User_width, User_height, User_seed, User_steps, User_cfg, User_prompt);

        // 發送確認訊息
        Player sender = PhotonNetwork.CurrentRoom.GetPlayer(senderId);
        photonView.RPC("ReceiveServerStatus", sender, "busy");
        clientID = senderId;
        if (sender != null)
        {
            photonView.RPC("ReceiveConfirmation", sender, "已收到您的訊息","good");
        }
        
    }
    [PunRPC]
    private void ReceiveServerStatus(string message)
    {
        Debug.Log($"收到伺服器狀態: {message}");
        UpdateStatus(message);
        status = message;
    }
    [PunRPC]
    private void ReceiveConfirmation(string message, string isbusy)
    {
        Debug.Log(message + isbusy);
        UpdateStatus(message);
        status = isbusy;
        if (status == "finish")
            image.color = Color.green;
        else
            image.color = Color.red;
    }
    [PunRPC]
    public void SendFileIds(string pngId, string latentId, string conditioningId)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        latestPngId = pngId;
        latestLatentId = latentId;
        latestConditioningId = conditioningId;
        Player sender = PhotonNetwork.CurrentRoom.GetPlayer(clientID);
        // 發送給所有其他玩家
        photonView.RPC("ReceiveFileIds", sender, pngId, latentId, conditioningId);
    }
    [PunRPC]
    private void BroadcastId(string id)
    {
        // 空實現，只為了防止錯誤
    }
    [PunRPC]
    private void ReceiveFileIds(string pngId, string latentId, string conditioningId, string vaeId, string isfinish)
    {
        Debug.Log($"Received file IDs - PNG: {pngId}, Latent: {latentId}, Conditioning: {conditioningId}, VAE: {vaeId}");
        UpdateStatus($"Received file IDs - PNG: {pngId}, Latent: {latentId}, Conditioning: {conditioningId}, VAE: {vaeId}");
        // 通知 FileDownloader 開始下載
        FindObjectOfType<FileDownloader>()?.StartDownload(pngId, latentId, conditioningId, vaeId);
        status = isfinish;
        Debug.Log(status);
    }
    private void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        UpdateStatus("Server connected");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        UpdateStatus("Searching for server...");
        // 加入大廳後會自動收到房間列表更新
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // 離開房間
        PhotonNetwork.LeaveRoom();
    }
    public override void OnJoinedRoom()
    {
        UpdateStatus($"Joined room，{PhotonNetwork.PlayerList.Length}  users");
        joinButton.gameObject.SetActive(false);
        isConnect = true;
        image.color = Color.green;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateStatus($"新裝置已連線，{PhotonNetwork.PlayerList.Length} 個使用者");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateStatus($"裝置已離線，{PhotonNetwork.PlayerList.Length} 個使用者");
    }
    #endregion
}