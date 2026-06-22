// Minimal SignalR JSON-protocol client for the public NewsHub.  It avoids an
// external CDN so realtime updates work on networks without Internet access.
(() => {
    const recordSeparator = "\u001e";
    const handlers = [];
    let socket;
    let connecting = false;
    let reconnectTimer;

    function scheduleReconnect() {
        clearTimeout(reconnectTimer);
        reconnectTimer = setTimeout(connect, 3000);
    }

    async function connect() {
        if (connecting || socket?.readyState === WebSocket.OPEN) return;
        connecting = true;

        try {
            const response = await fetch('/newsHub/negotiate?negotiateVersion=1', {
                method: 'POST',
                credentials: 'same-origin',
                headers: { 'Content-Type': 'application/json' }
            });
            if (!response.ok) throw new Error(`Hub negotiate failed (${response.status})`);

            const negotiation = await response.json();
            const scheme = window.location.protocol === 'https:' ? 'wss' : 'ws';
            socket = new WebSocket(`${scheme}://${window.location.host}/newsHub?id=${encodeURIComponent(negotiation.connectionToken)}`);

            socket.onopen = () => socket.send(`{"protocol":"json","version":1}${recordSeparator}`);
            socket.onmessage = event => {
                event.data.split(recordSeparator).filter(Boolean).forEach(message => {
                    const payload = JSON.parse(message);
                    if (payload.type === 1 && payload.target === 'NewsChanged') {
                        handlers.forEach(handler => handler(payload.arguments[0]));
                    }
                });
            };
            socket.onclose = scheduleReconnect;
            socket.onerror = () => socket.close();
        } catch (error) {
            console.warn('News realtime connection failed:', error);
            scheduleReconnect();
        } finally {
            connecting = false;
        }
    }

    window.newsHub = {
        subscribe(handler) {
            handlers.push(handler);
            connect();
            return () => {
                const index = handlers.indexOf(handler);
                if (index >= 0) handlers.splice(index, 1);
            };
        }
    };
})();
