(() => {
    if (typeof signalR === "undefined") {
        console.error("SignalR client library was not loaded.");
        return;
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/newsHub")
        .withAutomaticReconnect()
        .build();

    const esc = s => (s ?? "").toString()
        .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;").replace(/'/g, "&#39;");

    const EDIT_SVG = `<svg width="13" height="13" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round" viewBox="0 0 24 24"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/><path d="M18.5 2.5a2.12 2.12 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>`;
    const DELETE_SVG = `<svg width="13" height="13" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round" viewBox="0 0 24 24"><polyline points="3 6 5 6 21 6"/><path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6"/></svg>`;

    // Builds a <tr> identical to the server-rendered row so realtime rows look/behave the same.
    function buildRow(p) {
        const tags = (p.tagNames && p.tagNames.length)
            ? p.tagNames.map(t => `<span class="badge rounded-pill text-bg-light border fw-normal">${esc(t)}</span>`).join("")
            : `<span class="text-muted small">&mdash;</span>`;
        const status = p.status
            ? `<span class="badge rounded-pill text-bg-success">Active</span>`
            : `<span class="badge rounded-pill text-bg-warning">Inactive</span>`;
        return `
        <tr data-row-id="${esc(p.id)}">
            <td class="text-muted small font-monospace">${esc(p.id)}</td>
            <td>
                <div class="news-title">${esc(p.title)}</div>
                <div class="news-headline text-muted small">${esc(p.headline)}</div>
            </td>
            <td><span class="badge rounded-pill cat-badge">${esc(p.category)}</span></td>
            <td class="text-muted small text-nowrap">${esc(p.created)}</td>
            <td>${status}</td>
            <td><div class="d-flex flex-wrap gap-1 tag-cell">${tags}</div></td>
            <td class="text-end text-nowrap">
                <button type="button" class="btn btn-sm btn-outline-primary me-1" onclick="openEditModal(this)"
                        data-id="${esc(p.id)}" data-title="${esc(p.title)}" data-headline="${esc(p.headline)}"
                        data-content="${esc(p.content)}" data-source="${esc(p.source)}" data-category="${esc(p.categoryId)}"
                        data-status="${p.status ? "true" : "false"}" data-state="${esc(p.stateId)}" data-tags="${esc(p.tags)}">
                    ${EDIT_SVG} Edit
                </button>
                <button type="button" class="btn btn-sm btn-outline-danger" onclick="openDeleteModal('${esc(p.id)}')">
                    ${DELETE_SVG} Delete
                </button>
            </td>
        </tr>`;
    }

    function showToast() {
        const el = document.getElementById("newsRealtimeToast");
        if (el && window.bootstrap) bootstrap.Toast.getOrCreateInstance(el).show();
    }

    connection.on("NewsChanged", payload => {
        const tbody = document.getElementById("newsTableBody");
        if (!tbody) return; // not on the news management page

        const existing = tbody.querySelector(`tr[data-row-id="${payload.id}"]`);
        if (payload.action === "deleted") {
            if (existing) existing.remove();
        } else if (existing) {
            existing.outerHTML = buildRow(payload);            // updated in place
        } else {
            tbody.insertAdjacentHTML("afterbegin", buildRow(payload)); // newest first
            document.getElementById("emptyRow")?.remove();
        }
        showToast();
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
