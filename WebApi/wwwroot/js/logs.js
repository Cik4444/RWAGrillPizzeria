
const logTable = document.getElementById('logTable').querySelector('tbody');
const logCount = document.getElementById('logCount');
const fetchLogsButton = document.getElementById('fetchLogs');
const logoutButton = document.getElementById('logout');

document.addEventListener('DOMContentLoaded', () => {
    function checkAuth() {
        const token = localStorage.getItem('token');
        if (!token) {
            alert('Access denied. Please log in.');
            window.location.href = '/html/login.html';
            return false;
        }
        return true;
    }

    if (!checkAuth()) return;

    async function fetchLogs(n) {
        try {
            const response = await fetch(`http://localhost:5289/api/logs/get/${n}`);
            const data = await response.json();

            console.log("Logs fetched from server:", data);
            const logs = data.$values;
            if (!logs || logs.length === 0) {
                console.log("No logs found");
                alert("No logs found in the database.");
                return;
            }

            displayLogs(logs);
        } catch (error) {
            console.error("Error fetching logs:", error);
            alert("Error fetching logs. Please try again.");
        }
    }


    function displayLogs(logs) {
        logTable.innerHTML = '';
        logs.forEach(log => {
            const row = document.createElement('tr');
            row.innerHTML = `
            <td>${log.id}</td>
            <td>${new Date(log.timestamp).toLocaleString()}</td>
            <td>${log.level}</td>
            <td>${log.message}</td>
        `;
            logTable.appendChild(row);
        });
    }

    document.getElementById('logout').addEventListener('click', () => {
        localStorage.removeItem('token');
        alert('You have been logged out.');
        window.location.href = '/html/login.html';
    });

    fetchLogsButton.addEventListener('click', fetchLogs);

    fetchLogs();

});