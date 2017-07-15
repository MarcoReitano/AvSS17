using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CymaticLabs.Unity3D.Amqp.UI
{
    public class MessageListController : MonoBehaviour
    {
        public Transform Prefab;
        public Transform ContentParent;

        private static MessageListController Instance;

        private static List<MessageItemUI> messages = new List<MessageItemUI>();

        public void Awake()
        {
            MessageListController.Instance = this;
        }

        public static void AddMessage(AmqpSubscriptionBase subscription, IAmqpReceivedMessage message)
        {
            var messageItem = Instantiate(Instance.Prefab, Instance.ContentParent).GetComponent<MessageItemUI>();
            messageItem.Subscription = subscription;
            messageItem.Message = message;

            messageItem.refreshUI();
        }

        public static void RemoveMessage()
        {

        }
    }
}
