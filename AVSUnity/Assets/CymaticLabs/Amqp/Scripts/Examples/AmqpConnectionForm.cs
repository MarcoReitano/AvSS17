using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CymaticLabs.Unity3D.Amqp.UI
{
    /// <summary>
    /// Performs UI logic for the demo AMQP connection form.
    /// </summary>
    public class AmqpConnectionForm : MonoBehaviour
    {
        #region Inspector

        public Dropdown Connection;
        public Button ConnectButton;
        public Button DisconnectButton;

        public Dropdown ExchangeName;
        public InputField RoutingKey;
        public Button SubscribeButton;
        public Button UnsubscribeButton;

        public Dropdown PublishExchange;
        public InputField PublishRoutingKey;
        public InputField PublishMessage;
        public Button PublishButton;

        public InputField DeclareQueueName;
        public Button DeclareQueueButton;
        public Button DeleteQueueButton;

        public InputField SubscribeQueueName;
        public Button SubscribeQueueButton;
        public Button UnsubscribeQueueButton;

        public InputField SendMessageQueueName;
        public InputField SendMessageQueueMessage;
        public Button SendMessageToQueueButton;

        public Button AcknowledgeMessageButton;

        #endregion Inspector

        #region Fields

        // List of created exchange subscriptions
        List<AmqpExchangeSubscription> exSubscriptions;
        List<AmqpQueueSubscription> queueSubscriptions;

        // The current list of exchanges
        AmqpExchange[] exchanges;

        //Queue
        Queue<IAmqpReceivedMessage> queueMessages;

        #endregion Fields

        #region Properties

        #endregion Properties

        #region Methods

        #region Init

        private void Awake()
        {
            exSubscriptions = new List<AmqpExchangeSubscription>();
            queueSubscriptions = new List<AmqpQueueSubscription>();

            queueMessages = new Queue<IAmqpReceivedMessage>();

            if (Connection == null) Debug.LogError("AmqpConnectionForm.Connection is not assigned");
            if (ExchangeName == null) Debug.LogError("AmqpConnectionForm.ExchangeName is not assigned");
            if (RoutingKey == null) Debug.LogError("AmqpConnectionForm.RoutingKey is not assigned");
            if (SubscribeButton == null) Debug.LogError("AmqpConnectionForm.SubscribeButton is not assigned");
            if (UnsubscribeButton == null) Debug.LogError("AmqpConnectionForm.UnsubscribeButton is not assigned");
            if (PublishExchange == null) Debug.LogError("AmqpConnectionForm.PublishExchange is not assigned");
            if (PublishRoutingKey == null) Debug.LogError("AmqpConnectionForm.PublishRoutingKey is not assigned");
            if (PublishMessage == null) Debug.LogError("AmqpConnectionForm.PublishMessage is not assigned");
            if (PublishButton == null) Debug.LogError("AmqpConnectionForm.PublishButton is not assigned");

            if (DeclareQueueName == null) Debug.LogError("AmqpConnectionForm.DeclareQueueName is not assigned");
            if (DeclareQueueButton == null) Debug.LogError("AmqpConnectionForm.DeclareQueueButton is not assigned");
            if (DeleteQueueButton == null) Debug.LogError("AmqpConnectionForm.DeleteQueueButton is not assigned");
            

            if (SubscribeQueueName == null) Debug.LogError("AmqpConnectionForm.SubscribeQueueName is not assigned");
            if (SubscribeQueueButton == null) Debug.LogError("AmqpConnectionForm.SubscribeQueueButton is not assigned");
            if (UnsubscribeQueueButton == null) Debug.LogError("AmqpConnectionForm.UnsubscribeQueueButton is not assigned");
            
            if (SendMessageQueueName == null) Debug.LogError("AmqpConnectionForm.SendMessageQueueName is not assigned");
            if (SendMessageQueueMessage == null) Debug.LogError("AmqpConnectionForm.SendMessageQueue is not assigned");
            if (SendMessageToQueueButton == null) Debug.LogError("AmqpConnectionForm.SendMessageToQueue is not assigned");


            if (AcknowledgeMessageButton == null) Debug.LogError("AmqpConnectionForm.AcknowledgeMessageButton is not assigned");
            
    }

        private void Start()
        {
            AmqpClient.Instance.OnConnected.AddListener(HandleConnected);
            AmqpClient.Instance.OnDisconnected.AddListener(HandleDisconnected);
            AmqpClient.Instance.OnReconnecting.AddListener(HandleReconnecting);
            AmqpClient.Instance.OnBlocked.AddListener(HandleBlocked);
            AmqpClient.Instance.OnSubscribedToExchange.AddListener(HandleExchangeSubscribed);
            AmqpClient.Instance.OnUnsubscribedFromExchange.AddListener(HandleExchangeUnsubscribed);
            AmqpClient.Instance.OnSubscribedToQueue.AddListener(HandleQueueSubscribed);
            AmqpClient.Instance.OnUnsubscribedFromQueue.AddListener(HandleQueueUnsubscribed);

            // Populate the connections drop down
            foreach (var c in AmqpClient.GetConnections())
            {
                var option = new Dropdown.OptionData(c.Name);
                Connection.options.Add(option);
            }

            // Select the initial item in the dropdown
            for (var i = 0; i < Connection.options.Count; i++)
            {
                if (Connection.options[i].text == AmqpClient.Instance.Connection)
                {
                    Connection.value = i;
                    break;
                }
            }

            Connection.RefreshShownValue();
        }

        #endregion Init

        #region Connect

        /// <summary>
        /// Connects to the AMQP broker using the form's client connection values.
        /// </summary>
        public void Connect()
        {
            // Connect
            ExchangeName.options.Clear();
            PublishExchange.options.Clear();

            var connectionName = Connection.options[Connection.value].text;
            AmqpClient.Instance.Connection = connectionName;

            AmqpClient.Connect();
        }

        #endregion Connect

        #region Disconnect

        /// <summary>
        /// Disconnects the AMQP client.
        /// </summary>
        public void Disconnect()
        {
            // Disconnect
            AmqpClient.Disconnect();
        }

        #endregion Disconnect

        #region Subscribe

        /// <summary>
        /// Subscribes to the AMQP exchange subscription using the form's values.
        /// </summary>
        public void Subscribe()
        {
            // Validate args
            var isValid = true;

            var exchangeName = ExchangeName.options[ExchangeName.value].text;

            if (string.IsNullOrEmpty(exchangeName))
            {
                isValid = false;
                AmqpConsole.Color = Color.red;
                AmqpConsole.WriteLine("* Exchange Name cannot be blank");
                AmqpConsole.Color = null;
            }

            // Don't continue if values are invald
            if (!isValid) return;

            var exchangeType = AmqpExchangeTypes.Direct;

            // Find this exchange and get its exchange type
            foreach (var exchange in exchanges)
            {
                if (exchange.Name == exchangeName)
                {
                    exchangeType = exchange.Type;
                    break;
                }
            }

            var routingKey = RoutingKey.text;

            // Ensure this subscription doesn't already exist
            foreach (var sub in exSubscriptions)
            {
                if (sub.ExchangeName == exchangeName && sub.ExchangeType == exchangeType && sub.RoutingKey == routingKey)
                {
                    AmqpConsole.Color = new Color(1f, 0.5f, 0);
                    AmqpConsole.WriteLineFormat("Subscription already exists for exchange {0}:{1}", exchangeName, routingKey);
                    AmqpConsole.Color = null;
                    return;
                }
            }

            // Create the new subscription
            var subscription = new UnityAmqpExchangeSubscription(exchangeName, exchangeType, routingKey, null, UnityEventDebugExhangeMessageHandler);

            // Subscribe on the client
            AmqpClient.Subscribe(subscription);
        }

        #endregion Subscribe

        #region Unsubscribe

        /// <summary>
        /// Unsubscribes from the AMQP exchange subscription using the form's values.
        /// </summary>
        public void Unsubscribe()
        {
            // Validate args
            var isValid = true;

            var exchangeName = ExchangeName.options[ExchangeName.value].text;

            if (string.IsNullOrEmpty(exchangeName))
            {
                isValid = false;
                AmqpConsole.Color = Color.red;
                AmqpConsole.WriteLine("* Exchange Name cannot be blank");
                AmqpConsole.Color = null;
            }

            // Don't continue if values are invald
            if (!isValid) return;

            var exchangeType = AmqpExchangeTypes.Direct;

            // Find this exchange and get its exchange type
            foreach (var exchange in exchanges)
            {
                if (exchange.Name == exchangeName)
                {
                    exchangeType = exchange.Type;
                    break;
                }
            }

            var routingKey = RoutingKey.text;

            // Ensure this subscription already exists
            var subs = exSubscriptions.ToArray();

            foreach (var sub in subs)
            {
                if (sub.ExchangeName == exchangeName && sub.ExchangeType == exchangeType && sub.RoutingKey == routingKey)
                {
                    AmqpClient.Unsubscribe(sub);
                    exSubscriptions.Remove(sub);
                    return;
                }
            }

            AmqpConsole.Color = new Color(1f, 0.5f, 0);
            AmqpConsole.WriteLineFormat("Subscription not found for exchange {0}:{1}", exchangeName, routingKey);
            AmqpConsole.Color = null;
        }

        #endregion Unsubscribe

        #region Publish

        /// <summary>
        /// Publishes a message to the current exchange using the form's input values.
        /// </summary>
        public void Publish()
        {
            // Validate args
            var isValid = true;

            var exchangeName = PublishExchange.options[PublishExchange.value].text;

            if (string.IsNullOrEmpty(exchangeName))
            {
                isValid = false;
                AmqpConsole.Color = Color.red;
                AmqpConsole.WriteLine("* Exchange Name cannot be blank");
                AmqpConsole.Color = null;
            }

            var message = PublishMessage.text;

            if (string.IsNullOrEmpty(message))
            {
                isValid = false;
                AmqpConsole.Color = Color.red;
                AmqpConsole.WriteLine("* Message cannot be blank");
                AmqpConsole.Color = null;
            }

            // Don't continue if values are invald
            if (!isValid) return;

            var exchangeType = AmqpExchangeTypes.Direct;

            // Find this exchange and get its exchange type
            foreach (var exchange in exchanges)
            {
                if (exchange.Name == exchangeName)
                {
                    exchangeType = exchange.Type;
                    break;
                }
            }

            var routingKey = PublishRoutingKey.text;

            // Publish the message
            AmqpClient.Publish(exchangeName, routingKey, message);
            PublishMessage.text = null; // clear out message

            // Refocus the message area
            PublishMessage.Select();
            PublishMessage.ActivateInputField();
        }

        #endregion Publish

        #region Queue
        public void DeclareQueue()
        {
            Debug.Log("DeclareQueue");
            AmqpClient.DeclareQueue(DeclareQueueName.text);

            foreach (var q in AmqpClient.GetQueues())
                Debug.Log(q.Name);
        }

        public void DeleteQueue()
        {
            Debug.Log("DeleteQueue");
            AmqpClient.DeleteQueue(DeclareQueueName.text);

            foreach (var q in AmqpClient.GetQueues())
                Debug.Log(q.Name);
        }

        public void SubscribeQueue()
        {
            Debug.Log("SubscribeQueue");
            var queueName = SubscribeQueueName.text;

            // Ensure this subscription doesn't already exist
            foreach (var sub in queueSubscriptions)
            {
                if (sub.QueueName == queueName)
                {
                    AmqpConsole.Color = new Color(1f, 0.5f, 0);
                    AmqpConsole.WriteLineFormat("Subscription already exists for exchange {0}", queueName);
                    AmqpConsole.Color = null;
                    return;
                }
            }

            var subscription = new UnityAmqpQueueSubscription(SubscribeQueueName.text, true, null, 
                UnityEventDebugQueueMessageHandler);

            AmqpClient.Subscribe(subscription);
        }

        public void UnsubscribeQueue()
        {
            Debug.Log("UnsubscribeQueue");
            var subscription = new AmqpQueueSubscription();
            AmqpClient.Unsubscribe(subscription);
        }

        public void SendMessageToQueue()
        {
            Debug.Log("SendMessageToQueue");
            //AmqpClient.Publish
            AmqpClient.Publish(SendMessageQueueName.text, SendMessageQueueMessage.text);
        }

        public void AcknowledgeMessage()
        {
            var msg = queueMessages.Dequeue();

            AmqpClient.AcknowledgeMessage(msg.DeliveryTag);

            var payload = System.Text.Encoding.UTF8.GetString(msg.Body);
            AmqpConsole.Color = new Color(1f, 0.5f, 0);
            AmqpClient.Log("Message acknowledged: " + payload);
            AmqpConsole.Color = null;
        }

        #endregion

        #region Event Handlers

        // Handles a connection event
        void HandleConnected(AmqpClient client)
        {
            Connection.interactable = false;
            ConnectButton.interactable = false;
            DisconnectButton.interactable = true;

            // Query exchange list
            AmqpClient.GetExchangesAsync(FinishConnected);
        }

        // Finishes the connection event by receiving the async result of the exchange query and display the results in the drop down
        void FinishConnected(AmqpExchange[] exchangeList)
        {
            // Copy list locally
            exchanges = exchangeList;

            foreach (var exchange in exchanges)
            {
                if (exchange.Name == null || exchange.Name == "/") continue;
                var option = new Dropdown.OptionData(exchange.Name);
                ExchangeName.options.Add(option);
                PublishExchange.options.Add(option);
            }

            if (exchanges.Length > 0)
            {
                ExchangeName.RefreshShownValue();
                PublishExchange.RefreshShownValue();
            }

            // Enable UI
            ExchangeName.interactable = true;
            RoutingKey.interactable = true;
            SubscribeButton.interactable = true;
            UnsubscribeButton.interactable = true;

            PublishButton.interactable = true;
            PublishExchange.interactable = true;
            PublishMessage.interactable = true;
            PublishRoutingKey.interactable = true;

            DeclareQueueName.interactable = true;
            DeclareQueueButton.interactable = true;
            DeleteQueueButton.interactable = true;

            SubscribeQueueName.interactable = true;
            SubscribeQueueButton.interactable = true;
            UnsubscribeQueueButton.interactable = true;

            SendMessageQueueName.interactable = true;
            SendMessageQueueMessage.interactable = true;
            SendMessageToQueueButton.interactable = true;

            AcknowledgeMessageButton.interactable = true;
        }

        // Handles a disconnection event
        void HandleDisconnected(AmqpClient client)
        {
            Connection.interactable = true;
            ConnectButton.interactable = true;
            DisconnectButton.interactable = false;

            ExchangeName.interactable = false;
            RoutingKey.interactable = false;
            SubscribeButton.interactable = false;
            UnsubscribeButton.interactable = false;

            PublishButton.interactable = false;
            PublishExchange.interactable = false;
            PublishMessage.interactable = false;
            PublishRoutingKey.interactable = false;

            DeclareQueueName.interactable = false;
            DeclareQueueButton.interactable = false;
            DeleteQueueButton.interactable = false;

            SubscribeQueueName.interactable = false;
            SubscribeQueueButton.interactable = false;
            UnsubscribeQueueButton.interactable = false;

            SendMessageQueueName.interactable = false;
            SendMessageQueueMessage.interactable = false;
            SendMessageToQueueButton.interactable = false;

            AcknowledgeMessageButton.interactable = false;
    }

        // Handles a reconnecting event
        void HandleReconnecting(AmqpClient client)
        {

        }

        // Handles a blocked event
        void HandleBlocked(AmqpClient client)
        {

        }

        // Handles exchange subscribes
        void HandleExchangeSubscribed(AmqpExchangeSubscription subscription)
        {
            // Add it to the local list
            exSubscriptions.Add(subscription);
        }

        // Handles exchange unsubscribes
        void HandleExchangeUnsubscribed(AmqpExchangeSubscription subscription)
        {
            // Add it to the local list
            exSubscriptions.Remove(subscription);
        }

        // Handles queue subscribes
        void HandleQueueSubscribed(AmqpQueueSubscription subscription)
        {
            Debug.Log("HandleQueueSubscribed");
            queueSubscriptions.Add(subscription);
        }

        void HandleQueueUnsubscribed(AmqpQueueSubscription subscription)
        {
            Debug.Log("HandleQueueUnsubscribed");
            queueSubscriptions.Remove(subscription);
        }

        public void UnityEventDebugExhangeMessageHandler(AmqpExchangeSubscription subscription, IAmqpReceivedMessage message)
        {
            // Decode as text
            var payload = System.Text.Encoding.UTF8.GetString(message.Body);
            AmqpConsole.Color = new Color(1f, 0.5f, 0);
            AmqpClient.Log("Message received on {0}: {1}", subscription.ExchangeName + (!string.IsNullOrEmpty(message.RoutingKey) ? ":" + message.RoutingKey : ""), payload);
            AmqpConsole.Color = null;
        }

        /// <summary>
        /// A default message received handler useful for debugging.
        /// </summary>
        /// <param name="subscription">The subscription the message was received on.</param>
        /// <param name="message">The message that was received.</param>
        public void UnityEventDebugQueueMessageHandler(AmqpQueueSubscription subscription, IAmqpReceivedMessage message)
        {
            // Decode as text
            queueMessages.Enqueue(message);
            var payload = System.Text.Encoding.UTF8.GetString(message.Body);
            AmqpConsole.Color = new Color(1f, 0.5f, 0);
            AmqpClient.Log("Message received on {0}: {1}", subscription.QueueName, payload);
            AmqpConsole.Color = null;
            //client.Ack(message.DeliveryTag);
        }

        #endregion Event Handlers

        #endregion Methods
    }
}


