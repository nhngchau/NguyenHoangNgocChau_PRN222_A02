(() => {
    if (typeof signalR === "undefined") {
        console.error("SignalR client library was not loaded.");
        return;
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/newsHub")
        .withAutomaticReconnect()
        .build();

    connection.on("NewsChanged", () => {
        console.log("NewsChanged received");
        const toastElement = document.getElementById("newsRealtimeToast");
        if (toastElement) {
            bootstrap.Toast.getOrCreateInstance(toastElement).show();
            window.setTimeout(() => window.location.reload(), 1000);
            return;
        }
        window.location.reload();
    });

    connection.onreconnecting(error => console.warn("SignalR reconnecting:", error));
    connection.onreconnected(() => console.log("SignalR reconnected"));
    connection.onclose(error => console.error("SignalR connection closed:", error));

    async function startConnection() {
        try {
            await connection.start();
            console.log("SignalR connected");
        } catch (error) {
            console.error("SignalR NewsHub connection failed:", error);
            window.setTimeout(startConnection, 5000);
        }
    }

    startConnection();
})();
