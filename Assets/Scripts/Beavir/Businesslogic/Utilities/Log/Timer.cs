using UnityEngine;

namespace Beavir.Businesslogic.Utilities
{
    public class Timer : MonoBehaviour
    {
        private float startTime;
        private bool keepTiming;
        private float timer;
        [SerializeField] private bool isTrigger = false;
        private GameObject find;

        void Start()
        {
            if (isTrigger) find = transform.parent.parent.GetChild(0).GetChild(0).gameObject;
            StartTimer();
        }

        private void OnEnable()
        {
            if (!keepTiming && !isTrigger) ResumeTimer();
        }

        void Update()
        {

            if (keepTiming)
            {
                UpdateTime();
            }
        }

        void UpdateTime()
        {
            timer = Time.time - startTime;
        }

        float StopTimer()
        {
            keepTiming = false;
            return timer;
        }

        public void PauseTimer()
        {
            keepTiming = false;
        }

        void ResumeTimer()
        {
            keepTiming = true;
            startTime = Time.time - timer;
        }

        void StartTimer()
        {
            if (!isTrigger) keepTiming = true;
            startTime = Time.time;
        }

        string TimeToString(float t)
        {
            string minutes = ((int)t / 60).ToString();
            string seconds = (t % 60).ToString("f2");
            return minutes + ":" + seconds;
        }

        private void OnDisable()
        {
            PauseTimer();
        }

        private void OnDestroy()
        {
            if (timer != 0)
            {
                if (!isTrigger) Debug.Log(gameObject.name + ";" + TimeToString(StopTimer()));
                else Debug.Log(find.name + ";" + TimeToString(StopTimer()));
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isTrigger)
            {
                ResumeTimer();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (isTrigger)
            {
                PauseTimer();
            }
        }
    }
}
