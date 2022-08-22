using PlayerLoopProfiles;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class Connection 
{
    public static IEnumerator ActivateEntry(this PingEntry pEntry)
    {
        PlayerLoopManager.PreventProfileChange++;
        pEntry.Status = PingEntry.PingStatus.CONNECTING;

        UnityWebRequest request = UnityWebRequest.Get(pEntry.FullUrl);
        yield return SendWebRequest(request);

        bool success = IsCodeSuccess(request.responseCode);
        if (request.error != null && request.error.ToLower().Contains("timeout"))
        {
            pEntry.StatusCode = 408;
        }
        else
        {
            pEntry.StatusCode = request.responseCode;
        }

        if (success)
        {
            pEntry.Status = PingEntry.PingStatus.SUCCESS;
            if (SettingsData.Settings.VibrateOnSuccess)
            {
                Handheld.Vibrate();
            }
        }
        else
        {
            pEntry.Status = PingEntry.PingStatus.FAILURE;
            if (SettingsData.Settings.VibrateOnFailure)
            {
                Handheld.Vibrate();
            }
        }

        yield return new WaitForSecondsRealtime(2);
        pEntry.Status = PingEntry.PingStatus.INACTIVE;
        PlayerLoopManager.PreventProfileChange--;
    }

    public static IEnumerator SetConnectionStatus(this EntryEditor pEditor, string pAddress)
    {
        PlayerLoopManager.PreventProfileChange++;
        pEditor.Status = EntryEditor.ConnectionStatus.CONNECTING;
        UnityWebRequest request = UnityWebRequest.Get(pAddress);
        yield return SendWebRequest(request);

        bool success = IsCodeSuccess(request.responseCode);
        pEditor.Status = success ? EntryEditor.ConnectionStatus.SUCCESS : EntryEditor.ConnectionStatus.FAILURE;
        PlayerLoopManager.PreventProfileChange--;
    }

    private static IEnumerator SendWebRequest(UnityWebRequest pRequest)
    {
        pRequest.certificateHandler = new CertificateDummy();
        pRequest.timeout = SettingsData.Settings.Timeout;
        yield return pRequest.SendWebRequest();
    }

    private static bool IsCodeSuccess(long pCode)
    {
        if (pCode >= 200 && pCode < 300)
        {
            return true;
        }

        return false;
    }

    private class CertificateDummy : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] pData)
        {
            return true;
        }
    }
}
