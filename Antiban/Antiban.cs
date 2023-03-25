using System;
using System.Collections.Generic;
namespace Antiban
{
    public class Antiban
    {
        public Dictionary<int, EventMessage> Dictionary { get; set; } = new();
        public PriorityQueue<EventMessage, EventMessage> PriorityQueue { get; set; } = new(new EventMessageComparer());
        /// <summary>
        /// Добавление сообщений в систему, для обработки порядка сообщений
        /// </summary>
        /// <param name="eventMessage"></param>
        public void PushEventMessage(EventMessage eventMessage)
        {
            foreach (var dic in Dictionary)
            {
                if (dic.Value.Phone.Trim() == eventMessage.Phone.Trim())
                {
                    if (dic.Value.Priority == 1 && eventMessage.Priority == 1)
                    {
                        if (eventMessage.DateTime.Subtract(dic.Value.DateTime).TotalHours < 24)
                        {
                            eventMessage.DateTime = dic.Value.DateTime.AddHours(24);
                            break;
                        }
                    }
                    else
                    {
                        if (eventMessage.DateTime.Subtract(dic.Value.DateTime).TotalMinutes < 1)
                        {
                            eventMessage.DateTime = dic.Value.DateTime.AddMinutes(1);
                            break;
                        }
                    }
                }
                else
                {
                    if (eventMessage.DateTime.Subtract(dic.Value.DateTime).TotalSeconds < 10)
                    {
                        eventMessage.DateTime = dic.Value.DateTime.AddSeconds(10);
                        break;
                    }
                }
            }
            Dictionary.Add(eventMessage.Id, eventMessage);
            PriorityQueue.Enqueue(eventMessage, eventMessage);
        }
        /// <summary>
        /// Вовзращает порядок отправок сообщений
        /// </summary>
        /// <returns></returns>
        public List<AntibanResult> GetResult()
        {
            var result = new List<AntibanResult>();
            while (PriorityQueue.TryDequeue(out EventMessage? em, out _))
            {
                result.Add(new AntibanResult
                {
                    EventMessageId = em.Id,
                    SentDateTime = em.DateTime
                });
            }
            RepareQueue();
            return result;
        }
        public void RepareQueue()
        {
            foreach (var (_, val) in Dictionary)
            {
                PriorityQueue.Enqueue(val, val);
            }
        }
    }
    public class EventMessageComparer : IComparer<EventMessage>
    {
        public int Compare(EventMessage? x, EventMessage? y)
        {
            if (x is null || y is null)
            {
                throw new ArgumentException("Пустое значение параметра метода Compare");

            }
            return x.DateTime.CompareTo(y.DateTime);
        }
    }
}
