# CS2 Server Blocker 🌍🔒
<p align="center">
<img width="128" height="128" alt="image" src="https://github.com/user-attachments/assets/c97cb540-bf3a-49f3-9b74-142aa67fda20" />

</p>

**CS2 Server Blocker** is a lightweight, Windows-based tool designed for Counter-Strike 2 players who want to have full control over their matchmaking experience. It allows you to block specific server regions (firewall rules) to ensure you only play on servers with low ping or good language compatibility.

<img width="882" height="584" alt="image" src="https://github.com/user-attachments/assets/f6156cab-a134-43f5-a186-1e20d3c11447" />


## 🚀 Key Features

*   **Server Blocking**: Easily block or unblock specific Valve matchmaking regions via Windows Firewall.
*   **Smart Status**: Clear "Blocked", "Timeout", or "ms" indicators in the list.
*   **Auto-Refresh**: Optional setting to automatically refresh ping times every 5 seconds.
*   **Immersive UI**: Modern Dark/Light themes that integrate with Windows DWM (Dark Title Bars & Scrollbars).
*   **Multilingual**: Fully localized in English, Romanian, Russian, Spanish, Portuguese, and French with flag indicators.
*   **Auto-Update**: Automatically checks for new versions on GitHub and notifies you.
*   **Reset Function**: One-click button to clear all firewall rules and restore default settings.

## 📥 Installation

1.  Go to the [Releases](https://github.com/sorinalex21/CS2-Server-Blocker/releases) page.
2.  Download the latest `CS2ServerBlocker.exe` (or the `.zip` archive).
3.  **Run as Administrator**: The application needs administrator privileges to manage Windows Firewall rules.

## 🛠️ How to Use

1.  **Open the App**: Right-click `CS2ServerBlocker.exe` and select **Run as Administrator**.
2.  **Load Servers**: The app automatically fetches the server list. If not, click **Load Servers**.
3.  **Select Regions**:
    *   **Check the box** ☑️ next to a server to block it.
    *   **Uncheck** to allow playing on that server.
    *   Blocked servers appear **Red** 🔴, allowed servers appear **Green/Normal** 🟢.
4.  **IP Status Indicator**:
    *   The "IPs" column shows exactly how many IP ranges are blocked vs total available (e.g., `2/3`).
    *   If enabled in Settings, the app automatically fixes any mismatch (`2/3` -> `3/3`) on startup.
5.  **Apply Rules**: Click **Apply** to save your changes to the Windows Firewall.
6.  **Reset**: Use the **Reset** button to delete all rules created by the app.

## ⚙️ Settings

*   **Dark/Light Mode**: Toggle between themes.
*   **Auto-refresh ping**: Automatically pings servers every 5 seconds.
*   **Auto-update firewall rules**: If enabled (default), the app automatically updates your blocked regions with the latest IPs from Valve on startup. If disabled, you must manually click "Apply" when you see a mismatch (e.g., `2/3` blocked).
*   Select your preferred Language.

## ⚠️ Requirements

*   Windows 10 / 11
*   .NET Desktop Runtime (usually pre-installed on modern Windows)
*   **Administrator Privileges** (Required for firewall management)

## 🤝 Contributing

Contributions are welcome! Feel free to submit a Pull Request or open an Issue if you encounter any bugs.

## 📜 License

This project is open-source. Feel free to use and modify it.

---
*Developed by [sorinalex21](https://github.com/sorinalex21)*
