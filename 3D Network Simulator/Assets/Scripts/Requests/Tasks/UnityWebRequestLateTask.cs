using System;
using GNS3.GNSThread;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Requests.Tasks
{
    public class UnityWebRequestLateTask : IQueuedTask
    {
        private readonly Action _start;
        private readonly Action _finish;
        private AsyncOperation _operation;
        private UnityWebRequest _request;
        private readonly Func<UnityWebRequest> _requestCreateFunc;
        public bool IsSuccessful => _request.isDone;

        public UnityWebRequestLateTask(Func<UnityWebRequest> urlCreate, Action start, Action finish)
        {
            _start = () => InnerStart(start);
            _requestCreateFunc = urlCreate;
            _finish = finish;
            Guid = Guid.NewGuid();

            IsRunning = false;
        }

        public UnityWebRequestLateTask(Func<UnityWebRequest> urlCreate, Action start, Action finish,
            string notification)
        {
            _start = () => InnerStart(start);
            _requestCreateFunc = urlCreate;
            _finish = finish;

            NotificationOnStart = "[..] " + notification;
            NotificationOnSuccess = "[<color=green>OK</color>] " + notification;
            NotificationOnError = "[<color=red>FL</color>] " + notification;

            IsRunning = false;
        }


        private void InnerStart(Action outerStart)
        {
            _request = _requestCreateFunc.Invoke();
            _operation = _request.SendWebRequest();
            outerStart.Invoke();
        }

        public Guid Guid { get; }
        public bool IsRunning { get; private set; }
        public string NotificationOnStart { get; set; }
        public string NotificationOnSuccess { get; set; }
        public string NotificationOnError { get; set; }

        public void Start()
        {
            _start.Invoke();
            IsRunning = true;
        }

        public void Finish()
        {
            _finish.Invoke();
            IsRunning = false;
        }

        public AsyncOperation DoWork()
        {
            return _operation;
        }
    }
}