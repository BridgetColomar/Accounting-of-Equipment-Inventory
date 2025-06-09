![free-icon-man-14439135](https://github.com/user-attachments/assets/3f7a2f1e-d1d2-439a-8e5c-f716ddc99fd0)
# 📦 Equipment Inventory Management Module

**Модуль учета инвентаризации оборудования** предназначен для автоматизации учёта, контроля состояния, перемещений и инвентаризации оборудования на предприятии.  
## 🛠️ Используемые технологии

![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat&logo=dotnet&logoColor=white)
![WPF](https://img.shields.io/badge/WPF-5C2D91?style=flat&logo=windows&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/EF_Core-6DB33F?style=flat&logo=dotnet&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=flat&logo=sqlite&logoColor=white)
![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91?style=flat&logo=visual-studio&logoColor=white)

---

## 📚 Содержание
- [Введение](#введение)
- [Установка](#установка)
- [Библиотеки](#библиотеки)
- [Базы данных](#базы-данных)
- [XML Документация](#xml-документация)
- [Диаграммы сущностей](#диаграммы-сущностей)
- [Инструкция](#инструкция)

---

## 🧾 Введение

Программный модуль разработан для упрощения учёта оборудования, автоматизации инвентаризаций и отслеживания местоположения техники на предприятии.  
Система позволяет эффективно управлять категориями оборудования, назначать ответственных, фиксировать перемещения, формировать отчёты и разграничивать доступ пользователей по ролям.

---

## ⚙️ Установка

Для установки проекта выполните следующую команду в терминале:

```bash
git clone https://github.com/youruser/EquipmentInventoryManagementModule.git
cd EquipmentInventoryManagementModule
```

Затем установите зависимости и запустите проект через Visual Studio или командой:

```bash
dotnet restore
dotnet build
dotnet run
```

---

## 📦 Библиотеки

Проект использует следующие NuGet-пакеты:

- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Sqlite`
- `Microsoft.EntityFrameworkCore.Tools`
- `System.Data.SQLite`
- `ClosedXML` (для экспорта отчетов в Excel)
- `PdfSharp` / `iTextSharp` (для генерации PDF)

---

## 🗃️ Базы данных

Поддерживаются два варианта СУБД:
- **SQLite** – используется локально, по умолчанию;
- **Microsoft SQL Server** – для централизованного хранения данных в организации.

Конфигурация подключается через `DbContextFactory`.

---

## 📄 XML Документация

Включена XML-документация по основным классам и методам.

---

## 📊 Диаграммы сущностей

![image](https://github.com/user-attachments/assets/8f38457e-8d12-43a5-9306-558eb81fcdef)

---

## 📝 Инструкция

**Порядок работы с приложением:**
1. Авторизоваться под своей ролью: администратор, менеджер, оператор
2. Менеджер может добавлять, редактировать, удалять оборудование, а также формировать отчёты
3. Администратор управляет учётными записями сотрудников
4. Оператор имеет доступ к просмотру информации об оборудовании
5. Все действия фиксируются в базе данных

⚠️ **Важно:** соблюдайте интерфейсные подсказки и структуру ролей. Все данные проверяются автоматически.

