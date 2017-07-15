using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace CymaticLabs.Unity3D.Amqp.UI
{
    public class MessageItemUI : MonoBehaviour {

        #region Inspector
        public static Transform prefab;

        public Image Background;

        public Text Payload;

        public Text ExchangeName;
        public Text QueueName;
        public Text RoutingKey;
        public Text DeliveryTag;
        public Text Redelivered;
        public Text ConsumerTag;

        public Button AckButton;
        public Button MultiAckButton;
        #endregion

        #region Fields
        IAmqpReceivedMessage message;
        AmqpSubscriptionBase subscription;
        #endregion

        #region Properties
        public IAmqpReceivedMessage Message
        {
            get { return message; }
            set
            {
                message = value;
            }
        }
        public AmqpSubscriptionBase Subscription
        {
            get { return subscription; }
            set
            {
                subscription = value;
            }
        }
        #endregion

        #region Methods

        #region Init
        private void Awake()
        {
            if (Background == null)
                Debug.LogError("MessageItemUI.Background is not assigned");

            if (Payload == null)
                Debug.LogError("MessageItemUI.Payload is not assigned");

            if (ExchangeName == null)
                Debug.LogError("MessageItemUI.ExchangeName is not assigned");
            if (QueueName == null)
                Debug.LogError("MessageItemUI.QueueName is not assigned");
            if (RoutingKey == null)
                Debug.LogError("MessageItemUI.RoutingKey is not assigned");
            if (DeliveryTag == null)
                Debug.LogError("MessageItemUI.DeliveryTag is not assigned");
            if (Redelivered == null)
                Debug.LogError("MessageItemUI.Redelivered is not assigned");
            if (ConsumerTag == null)
                Debug.LogError("MessageItemUI.ConsumerTag is not assigned");

            if (AckButton == null)
                Debug.LogError("MessageItemUI.AckButton is not assigned");
            if (MultiAckButton == null)
                Debug.LogError("MessageItemUI.MultiAckButton is not assigned");
        }

        private void Start()
        {

        }
        #endregion

        #region Update
        //private void Update()
        //{

        //}
        #endregion

        public void refreshUI()
        {
            Payload.text = System.Text.Encoding.UTF8.GetString(message.Body);

            ExchangeName.text = message.Exchange;

            if (subscription is AmqpQueueSubscription)
                QueueName.text = ((AmqpQueueSubscription)subscription).QueueName;
            else QueueName.text = "";

            RoutingKey.text = message.RoutingKey;
            DeliveryTag.text = message.DeliveryTag.ToString();
            Redelivered.text = message.Redelivered.ToString();
            ConsumerTag.text = message.ConsumerTag;
    }

        public void AckMessage()
        {
            try
            {
                AmqpClient.BasicAck(message.DeliveryTag, false);

                var payload = System.Text.Encoding.UTF8.GetString(message.Body);
                AmqpConsole.Color = new Color(1f, 0.5f, 0);
                AmqpClient.Log("This message was acknowledged: " + payload);
                AmqpConsole.Color = null;
            }
            catch (Exception ex)
            {
                AmqpConsole.Color = new Color(1f, 0.5f, 0);
                AmqpClient.Log("ERROR: " + ex.Message);
                AmqpConsole.Color = null;
            }
            Background.color = new Color32(45, 210, 39, 81);
        }

        public void MultiAckMessage()
        {
            try
            {
                AmqpClient.BasicAck(message.DeliveryTag, true);

                var payload = System.Text.Encoding.UTF8.GetString(message.Body);
                AmqpConsole.Color = new Color(1f, 0.5f, 0);
                AmqpClient.Log("This message (and all previous unacknowledged ones) were acknowledged: " + payload);
                AmqpConsole.Color = null;
            }
            catch (Exception ex)
            {
                AmqpConsole.Color = new Color(1f, 0.5f, 0);
                AmqpClient.Log("ERROR: " + ex.Message);
                AmqpConsole.Color = null;
            }
            Background.color = new Color32(45, 210, 39, 81);
        }

        public void ActivateUI(bool active)
        {
            //Payload.;

            //ExchangeName;
            //QueueName;
            //RoutingKey;
            //DeliveryTag;
            //Redelivered;
            //ConsumerTag;

            AckButton.interactable = active;
            MultiAckButton.interactable = active;
        }
        #endregion
    }
}
