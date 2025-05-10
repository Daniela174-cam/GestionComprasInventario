# 📦 Sistema de Gestión de Compras e Inventario

Este proyecto es una aplicación de consola desarrollada en **C# (.NET 9)** que permite la gestión de compras, ventas, movimientos de caja, productos y planes promocionales, conectándose a una base de datos **PostgreSQL**. Utiliza una arquitectura modular basada en el patrón **Repository** y principios de diseño limpio.

---

## 🚀 Características principales

- 📥 Registro de compras y actualización automática del stock.
- 💸 Registro de ventas y control de stock disponible.
- 🧾 Manejo de movimientos de caja por tipo de operación.
- 🏷️ Creación y consulta de planes promocionales activos.
- 📊 Consulta de balances diarios.
- 🧩 Integración con PostgreSQL mediante `Npgsql`.
- 📂 Arquitectura separada en capas (Core, Infrastructure, ConsoleApp).

---

## 🛠️ Tecnologías utilizadas

- **Lenguaje:** C# (.NET 9)
- **Base de datos:** PostgreSQL
- **ORM / Driver:** Npgsql
- **Arquitectura:** MVC + Repository Pattern
- **IDE recomendado:** Visual Studio / VS Code

---

## 🗃️ Estructura del proyecto

---

## ⚙️ Configuración y ejecución

### 📌 Requisitos previos

- [.NET SDK 9+](https://dotnet.microsoft.com/)
- [PostgreSQL 14+](https://www.postgresql.org/)
- [DBeaver](https://dbeaver.io/) o cliente similar para gestión de BD

### 🔧 Configura la conexión

Edita el archivo `DBContext.cs` para establecer tu conexión a PostgreSQL:

```csharp
_connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=tu_clave;Database=GestionCompras");
