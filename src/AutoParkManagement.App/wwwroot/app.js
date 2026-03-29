document.addEventListener('DOMContentLoaded', () => {
    loadDrivers();

    document.getElementById('driverForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        await saveDriver();
    });
});

// GET: Получение всех водителей
async function loadDrivers() {
    try {
        const response = await fetch('/api/drivers/');
        if (response.ok) {
            const drivers = await response.json();
            const tbody = document.querySelector('#driversTable tbody');
            tbody.innerHTML = '';

            drivers.forEach(d => {
                tbody.innerHTML += `
                    <tr>
                        <td>${d.id}</td>
                        <td>${d.name}</td>
                        <td>${d.rating}</td>
                        <td>${d.carId}</td>
                        <td>
                            <button class="edit-btn" onclick="editDriver(${d.id}, '${d.name}', ${d.rating}, ${d.carId})">Ред.</button>
                            <button class="delete-btn" onclick="deleteDriver(${d.id})">Удалить</button>
                        </td>
                    </tr>`;
            });
        }
    } catch (error) {
        console.error("Ошибка загрузки водителей:", error);
    }
}

// POST / PUT: Сохранение или обновление водителя
async function saveDriver() {
    const id = document.getElementById('driverId').value;
    const driver = {
        name: document.getElementById('driverName').value,
        rating: parseInt(document.getElementById('driverRating').value),
        carId: parseInt(document.getElementById('driverCarId').value)
    };

    const isEdit = id !== "";
    if (isEdit) {
        driver.id = parseInt(id);
    }

    const url = isEdit ? `/api/drivers/${id}` : '/api/drivers/';
    const method = isEdit ? 'PUT' : 'POST';

    const response = await fetch(url, {
        method: method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(driver)
    });

    if (response.ok) {
        resetForm();
        await loadDrivers();
    } else {
        alert("Ошибка при сохранении!");
    }
}

// DELETE: Удаление водителя
async function deleteDriver(id) {
    if (confirm("Вы уверены, что хотите удалить водителя?")) {
        const response = await fetch(`/api/drivers/${id}`, { method: 'DELETE' });
        if (response.ok) {
            await loadDrivers();
        }
    }
}

// Подготовка формы для редактирования
function editDriver(id, name, rating, carId) {
    document.getElementById('driverId').value = id;
    document.getElementById('driverName').value = name;
    document.getElementById('driverRating').value = rating;
    document.getElementById('driverCarId').value = carId;

    document.getElementById('saveBtn').innerText = "Сохранить изменения";
    document.getElementById('cancelBtn').style.display = "inline-block";
}

// Сброс формы в исходное состояние
function resetForm() {
    document.getElementById('driverId').value = "";
    document.getElementById('driverForm').reset();
    document.getElementById('saveBtn').innerText = "Добавить водителя";
    document.getElementById('cancelBtn').style.display = "none";
}