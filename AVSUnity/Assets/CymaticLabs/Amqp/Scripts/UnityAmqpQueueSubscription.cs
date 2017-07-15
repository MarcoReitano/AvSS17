using UnityEngine.Events;

namespace CymaticLabs.Unity3D.Amqp
{
    /// <summary>
    /// Unity-specific AMQP queue subscription that exposes Unity events for the
    /// subscription's message received handler.
    /// </summary>
    [System.Serializable]
    public class UnityAmqpQueueSubscription : AmqpQueueSubscription
    {
        /// <summary>
        /// Occurs when a message is received by the subscription.
        /// </summary>
        public AmqpQueueMessageReceivedUnityEvent OnMessageReceived;

        /// <summary>
        /// Creates a new queue subscription.
        /// </summary>
        /// <param name="queueName">The name of the queue to subscribe to.</param>
        /// <param name="useAck">Wheter to acknowledge messages. </param>
        /// <param name="handler">The message received hanlder to use with the subscription.</param>
        /// <param name="unityHandler">The Unity message received handler to use with the subscription</param>
        public UnityAmqpQueueSubscription(string queueName, bool useAck, AmqpQueueMessageReceivedEventHandler handler, UnityAction<AmqpQueueSubscription, IAmqpReceivedMessage> unityHandler)
            : this("Unity Queue Subscription", queueName, useAck, handler, unityHandler)
        { }

        /// <summary>
        /// Creates a new queue subscription.
        /// </summary>
        /// <param name="name">The name to give the subscription.</param>
        /// <param name="queueName">The name of the queue to subscribe to.</param>
        /// <param name="useAck">Wheter to acknowledge messages. </param>
        /// <param name="handler">The message received hanlder to use with the subscription.</param>
        /// <param name="unityHandler">The Unity message received handler to use with the subscription</param>
        public UnityAmqpQueueSubscription(string name, string queueName, bool useAck, AmqpQueueMessageReceivedEventHandler handler, UnityAction<AmqpQueueSubscription, IAmqpReceivedMessage> unityHandler)
            : base(name, queueName, useAck, handler)
        {
            OnMessageReceived = new AmqpQueueMessageReceivedUnityEvent();
            OnMessageReceived.AddListener(unityHandler);
        }
    }
}
