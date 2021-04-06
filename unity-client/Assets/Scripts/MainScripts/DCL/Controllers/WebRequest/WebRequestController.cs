using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DCL
{
    /// <summary>
    /// This class manage all our custom WebRequests types.
    /// </summary>
    public interface IWebRequestController
    {
        /// <summary>
        /// Initialize the controller with all the request types injected.
        /// </summary>
        /// <param name="genericWebRequest"></param>
        /// <param name="assetBundleWebRequest"></param>
        /// <param name="textureWebRequest"></param>
        /// <param name="audioWebRequest"></param>
        void Initialize(
            IWebRequest genericWebRequest,
            IWebRequestAssetBundle assetBundleWebRequest,
            IWebRequest textureWebRequest,
            IWebRequestAudio audioWebRequest);

        /// <summary>
        /// Download data from a url.
        /// </summary>
        /// <param name="url">Url where to make the request.</param>
        /// <param name="OnSuccess">This action will be executed if the request successfully finishes and it includes the request with the data downloaded.</param>
        /// <param name="OnFail">This action will be executed if the request fails.</param>
        /// <param name="requestAttemps">Number of attemps for re-trying failed requests.</param>
        /// <param name="timeout">Sets the request to attempt to abort after the configured number of seconds have passed (0 = no timeout).</param>
        WebRequestAsyncOperation Get(
            string url,
            Action<UnityWebRequest> OnSuccess = null,
            Action<string> OnFail = null,
            int requestAttemps = 3,
            int timeout = 0);

        /// <summary>
        /// Download an Asset Bundle from a url.
        /// </summary>
        /// <param name="url">Url where to make the request.</param>
        /// <param name="OnSuccess">This action will be executed if the request successfully finishes and it includes the request with the data downloaded.</param>
        /// <param name="OnFail">This action will be executed if the request fails.</param>
        /// <param name="requestAttemps">Number of attemps for re-trying failed requests.</param>
        /// <param name="timeout">Sets the request to attempt to abort after the configured number of seconds have passed (0 = no timeout).</param>
        WebRequestAsyncOperation GetAssetBundle(
            string url,
            Action<UnityWebRequest> OnSuccess = null,
            Action<string> OnFail = null,
            int requestAttemps = 3,
            int timeout = 0);

        /// <summary>
        /// Download a texture from a url.
        /// </summary>
        /// <param name="url">Url where to make the request.</param>
        /// <param name="OnSuccess">This action will be executed if the request successfully finishes and it includes the request with the data downloaded.</param>
        /// <param name="OnFail">This action will be executed if the request fails.</param>
        /// <param name="requestAttemps">Number of attemps for re-trying failed requests.</param>
        /// <param name="timeout">Sets the request to attempt to abort after the configured number of seconds have passed (0 = no timeout).</param>
        WebRequestAsyncOperation GetTexture(
            string url,
            Action<UnityWebRequest> OnSuccess = null,
            Action<string> OnFail = null,
            int requestAttemps = 3,
            int timeout = 0);

        /// <summary>
        /// Download an audio clip from a url.
        /// </summary>
        /// <param name="url">Url where to make the request.</param>
        /// <param name="audioType">Type of audio that will be requested.</param>
        /// <param name="OnSuccess">This action will be executed if the request successfully finishes and it includes the request with the data downloaded.</param>
        /// <param name="OnFail">This action will be executed if the request fails.</param>
        /// <param name="requestAttemps">Number of attemps for re-trying failed requests.</param>
        /// <param name="timeout">Sets the request to attempt to abort after the configured number of seconds have passed (0 = no timeout).</param>
        WebRequestAsyncOperation GetAudioClip(
            string url,
            AudioType audioType,
            Action<UnityWebRequest> OnSuccess = null,
            Action<string> OnFail = null,
            int requestAttemps = 3,
            int timeout = 0);

        /// <summary>
        /// Abort and clean all the ongoing web requests.
        /// </summary>
        void Dispose();
    }

    public class WebRequestController : IWebRequestController
    {
        public static WebRequestController i { get; private set; }

        private IWebRequest genericWebRequest;
        private IWebRequestAssetBundle assetBundleWebRequest;
        private IWebRequest textureWebRequest;
        private IWebRequestAudio audioClipWebRequest;
        private List<UnityWebRequest> ongoingWebRequests = new List<UnityWebRequest>();

        public static WebRequestController Create()
        {
            WebRequestController newWebRequestController = new WebRequestController();

            newWebRequestController.Initialize(
                genericWebRequest: new WebRequest(),
                assetBundleWebRequest: new WebRequestAssetBundle(),
                textureWebRequest: new WebRequestTexture(),
                audioClipWebRequest: new WebRequestAudio());

            return newWebRequestController;
        }

        public void Initialize(
            IWebRequest genericWebRequest,
            IWebRequestAssetBundle assetBundleWebRequest,
            IWebRequest textureWebRequest,
            IWebRequestAudio audioClipWebRequest)
        {
            i = this;

            this.genericWebRequest = genericWebRequest;
            this.assetBundleWebRequest = assetBundleWebRequest;
            this.textureWebRequest = textureWebRequest;
            this.audioClipWebRequest = audioClipWebRequest;
        }

        public WebRequestAsyncOperation Get(
            string url,
            Action<UnityWebRequest> OnSuccess = null,
            Action<string> OnFail = null,
            int requestAttemps = 3,
            int timeout = 0)
        {
            return SendWebRequest(genericWebRequest, url, OnSuccess, OnFail, requestAttemps, timeout);
        }

        public WebRequestAsyncOperation GetAssetBundle(
            string url,
            Action<UnityWebRequest> OnSuccess = null,
            Action<string> OnFail = null,
            int requestAttemps = 3,
            int timeout = 0)
        {
            return SendWebRequest(assetBundleWebRequest, url, OnSuccess, OnFail, requestAttemps, timeout);
        }

        public WebRequestAsyncOperation GetAssetBundle(
            string url,
            Hash128 hash,
            Action<UnityWebRequest> OnSuccess = null,
            Action<string> OnFail = null,
            int requestAttemps = 3,
            int timeout = 0)
        {
            assetBundleWebRequest.SetHash(hash);
            return SendWebRequest(assetBundleWebRequest, url, OnSuccess, OnFail, requestAttemps, timeout);
        }

        public WebRequestAsyncOperation GetTexture(
            string url,
            Action<UnityWebRequest> OnSuccess = null,
            Action<string> OnFail = null,
            int requestAttemps = 3,
            int timeout = 0)
        {
            return SendWebRequest(textureWebRequest, url, OnSuccess, OnFail, requestAttemps, timeout);
        }

        public WebRequestAsyncOperation GetAudioClip(
            string url,
            AudioType audioType,
            Action<UnityWebRequest> OnSuccess = null,
            Action<string> OnFail = null,
            int requestAttemps = 3,
            int timeout = 0)
        {
            audioClipWebRequest.SetAudioType(audioType);
            return SendWebRequest(audioClipWebRequest, url, OnSuccess, OnFail, requestAttemps, timeout);
        }

        private WebRequestAsyncOperation SendWebRequest<T>(
            T requestType,
            string url,
            Action<UnityWebRequest> OnSuccess,
            Action<string> OnFail,
            int requestAttemps,
            int timeout) where T : IWebRequest
        {
            int remainingAttemps = Mathf.Clamp(requestAttemps, 1, requestAttemps);

            UnityWebRequest request = requestType.CreateWebRequest(url);
            request.timeout = timeout;

            WebRequestAsyncOperation resultOp = new WebRequestAsyncOperation(request);
            ongoingWebRequests.Add(request);

            UnityWebRequestAsyncOperation requestOp = request.SendWebRequest();
            requestOp.completed += (asyncOp) =>
            {
                if (!resultOp.isDisposed)
                {
                    if (request.WebRequestSucceded())
                    {
                        OnSuccess?.Invoke(request);
                        resultOp.SetAsCompleted();
                    }
                    else if (!request.WebRequestAborted() && request.WebRequestServerError())
                    {
                        remainingAttemps--;
                        if (remainingAttemps > 0)
                        {
                            Debug.LogWarning($"Retrying web request: {url} ({remainingAttemps} attemps remaining)");
                            resultOp = SendWebRequest(requestType, url, OnSuccess, OnFail, remainingAttemps, timeout);
                        }
                        else
                        {
                            OnFail?.Invoke(request.error);
                            resultOp.SetAsCompleted();
                        }
                    }
                    else
                    {
                        OnFail?.Invoke(request.error);
                        resultOp.SetAsCompleted();
                    }
                }

                ongoingWebRequests.Remove(request);
            };

            return resultOp;
        }

        public void Dispose()
        {
            foreach (var webRequest in ongoingWebRequests)
            {
                webRequest.Dispose();
            }
        }
    }
}