<div align="center">

<h1>📋 SGESWeb</h1>
<p><strong>Sistema de Gestión de Eventos SENA — Rama <code>develop</code></strong></p>

<p>
  <img src="https://img.shields.io/badge/ASP.NET%20MVC-5-512BD4?style=for-the-badge&logo=dotnet" alt="ASP.NET MVC"/>
  <img src="https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge&logo=bootstrap" alt="Bootstrap 5"/>
  <img src="https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver" alt="SQL Server"/>
  <img src="https://img.shields.io/badge/Git%20Flow-activo-F05032?style=for-the-badge&logo=git" alt="Git Flow"/>
</p>

</div>

---

## 🗂 Tabla de contenido

1. [¿Qué es este proyecto?](#-qué-es-este-proyecto)
2. [¿Cómo funciona ASP.NET MVC? (explicado simple)](#-cómo-funciona-aspnet-mvc-explicado-simple)
3. [Estructura del proyecto](#-estructura-del-proyecto)
4. [Archivos clave — qué hace cada uno](#-archivos-clave--qué-hace-cada-uno)
5. [Base de datos](#-base-de-datos)
6. [Flujo de una petición de principio a fin](#-flujo-de-una-petición-de-principio-a-fin)
7. [Componentes visuales reutilizables (Shared/)](#-componentes-visuales-reutilizables-shared)
8. [Cómo agregar una nueva funcionalidad](#-cómo-agregar-una-nueva-funcionalidad)
9. [Flujo de trabajo Git (Git Flow)](#-flujo-de-trabajo-git-git-flow)
10. [Configuración inicial](#-configuración-inicial)

---

## 🎯 ¿Qué es este proyecto?

**SGESWeb** es una aplicación web para el **SENA** que permite gestionar eventos institucionales.

<table>
<tr>
<th>¿Quién la usa?</th>
<th>¿Qué puede hacer?</th>
</tr>
<tr>
<td>👤 <strong>Administrador</strong></td>
<td>Crear y gestionar eventos</td>
</tr>
<tr>
<td>🎓 <strong>Aprendiz</strong></td>
<td>Ver el listado de eventos disponibles</td>
</tr>
</table>

Cuando un usuario inicia sesión, la aplicación detecta su rol automáticamente y lo redirige a la pantalla correcta.

---

## 🧩 ¿Cómo funciona ASP.NET MVC? (explicado simple)

Si nunca has trabajado con este framework, piensa en él así:

```
El usuario hace clic o escribe una URL
         ↓
   CONTROLADOR recibe la petición
   (es como el "gerente" que decide qué hacer)
         ↓
   Habla con el MODELO
   (que tiene la lógica y habla con la base de datos)
         ↓
   El controlador manda los datos a la VISTA
   (que es el HTML que ve el usuario)
```

El patrón **MVC** significa:

| Letra | Nombre | Rol en SGESWeb |
|-------|--------|----------------|
| **M** | Model | Clases C# que representan datos y hacen consultas SQL (`EventoModel`, `EventoDAO`, `AuthDAO`…) |
| **V** | View | Archivos `.cshtml` con el HTML + lógica de presentación (`CrearEvento.cshtml`, `Login.cshtml`…) |
| **C** | Controller | Clases C# que reciben URLs, llaman al modelo y retornan una vista (`EventoController`, `AuthController`…) |

### ¿Cómo se conecta una URL con el código?

La URL `http://localhost/Evento/CrearEvento` se traduce así:

```
/Evento        → busca EventoController.cs
/CrearEvento   → ejecuta el método CrearEvento() dentro de ese controlador
```

Esto está definido en [`App_Start/RouteConfig.cs`](SGES.Web/SGES.Web/App_Start/RouteConfig.cs):

```csharp
url: "{controller}/{action}/{id}"
//     ↑               ↑       ↑
//  EventoController  CrearEvento  (opcional)
```

La ruta por defecto (página de inicio) es `Auth/Login`.

---

## 📁 Estructura del proyecto

```
SGESWeb/
│
├── 📄 README.md                        ← Este archivo
├── 📄 .gitignore
├── 📄 LICENSE
│
└── SGES.Web/
    └── SGES.Web/                       ← Proyecto principal ASP.NET MVC
        │
        ├── 🗂 App_Start/               ← Configuración que se carga al arrancar
        │   ├── BundleConfig.cs         ← Agrupa y minifica CSS/JS
        │   ├── FilterConfig.cs         ← Filtros globales (ej: manejo de errores)
        │   └── RouteConfig.cs          ← Reglas de URL
        │
        ├── 🗂 Controllers/             ← Controladores (reciben peticiones HTTP)
        │   ├── AuthController.cs       ← Login / Logout
        │   ├── EventoController.cs     ← Crear evento / Listado
        │   └── HomeController.cs       ← Páginas de prueba (About, Contact)
        │
        ├── 🗂 Models/                  ← Datos y acceso a BD
        │   ├── Conexión.cs             ← Abre la conexión SQL Server
        │   ├── LoginModel.cs           ← Datos del formulario de login
        │   ├── UsuarioSesion.cs        ← Datos del usuario en sesión
        │   ├── EventoModel.cs          ← Estructura de un evento
        │   ├── EventoDAO.cs            ← Consultas SQL de eventos
        │   └── AuthDAO.cs              ← Consultas SQL de autenticación
        │
        ├── 🗂 Views/                   ← Plantillas HTML (.cshtml)
        │   ├── _ViewStart.cshtml       ← Indica que todas las vistas usan _Layout
        │   │
        │   ├── 🗂 Auth/
        │   │   └── Login.cshtml        ← Pantalla de inicio de sesión
        │   │
        │   ├── 🗂 Evento/
        │   │   ├── CrearEvento.cshtml  ← Formulario para crear un evento
        │   │   └── Listado.cshtml      ← Tabla de eventos disponibles
        │   │
        │   ├── 🗂 Home/                ← Vistas de prueba (no se usan en producción)
        │   │
        │   └── 🗂 Shared/             ← Componentes reutilizables y layout
        │       ├── _Layout.cshtml      ← Plantilla base (navbar + footer)
        │       ├── _TextBox.cshtml     ← Campo de texto
        │       ├── _TextArea.cshtml    ← Campo de texto multilínea
        │       ├── _ComboBox.cshtml    ← Lista desplegable
        │       ├── _BotonGuardar.cshtml
        │       ├── _BotonCancelar.cshtml
        │       ├── _BotonEliminar.cshtml
        │       ├── _Card.cshtml        ← Contenedor tarjeta Bootstrap
        │       ├── _TablaBasica.cshtml ← Tabla de datos reutilizable
        │       ├── COMPONENTES.md      ← 📖 Guía detallada de cada componente
        │       └── site.css            ← Estilos personalizados del proyecto
        │
        ├── 🗂 Database/                ← Scripts SQL
        │   ├── Shema.sql               ← Crea todas las tablas
        │   └── Data.sql                ← Datos de prueba
        │
        ├── 🗂 Content/                 ← Archivos CSS (Bootstrap + Site.css)
        ├── 🗂 Scripts/                 ← Archivos JS (jQuery, Bootstrap, Validate)
        │
        ├── Global.asax                 ← Punto de arranque de la aplicación
        └── Web.config                  ← Configuración: cadena de conexión BD, etc.
```

---

## 📄 Archivos clave — qué hace cada uno

### `Web.config`
Es el archivo de configuración principal. Lo más importante que contiene es la **cadena de conexión** a la base de datos:

```xml
<add name="SGESConnection"
     connectionString="Data Source=.;Initial Catalog=SGES;Integrated Security=True;"
     providerName="System.Data.SqlClient" />
```

> **¿Qué significa?** `Data Source=.` significa "SQL Server local". `Initial Catalog=SGES` es el nombre de la base de datos. Si tu SQL Server tiene un nombre distinto, cámbialo aquí.

---

### `Models/Conexión.cs`
Se encarga de **abrir la conexión** con SQL Server usando la cadena del `Web.config`. Todos los DAOs la usan.

```csharp
public SqlConnection ObtenerConexion()
{
    return new SqlConnection(cadenaConexion);
}
```

---

### `Models/UsuarioSesion.cs`
Guarda los datos del usuario que está logueado:

```csharp
public class UsuarioSesion
{
    public int    Id     { get; set; }
    public string Nombre { get; set; }
    public string Tipo   { get; set; }  // "Administrador" o "Aprendiz"
}
```

Se almacena en `Session["Usuario"]` al iniciar sesión y se usa en toda la aplicación para saber quién está conectado y qué puede ver.

---

### `Models/AuthDAO.cs`
Hace las consultas SQL para autenticar. Busca primero en la tabla `Usuario` (administradores) y si no encuentra, en `Aprendiz`. Retorna un `UsuarioSesion` con nombre y tipo, o `null` si las credenciales son incorrectas.

---

### `Models/EventoModel.cs`
Representa un evento de la base de datos:

| Propiedad | Tipo | Columna en BD |
|-----------|------|---------------|
| `IdEvento` | `int` | `idEvento` |
| `NombreEvento` | `string` | `nombreEvento` |
| `TipoEvento` | `string` | `tipoEvento` |
| `FechaHoraInicio` | `DateTime` | `fechaHoraInicio` |
| `FechaHoraFin` | `DateTime` | `fechaHoraFin` |
| `IdUser` | `int` | `idUser` (FK → Usuario) |

---

### `Models/EventoDAO.cs`
Contiene las operaciones de base de datos para eventos:

- **`ObtenerEventos()`** → `SELECT * FROM Eventos` y retorna una lista de `EventoModel`
- **`InsertarEvento(evento)`** → `INSERT INTO Eventos` con los datos del modelo

---

### `Controllers/AuthController.cs`

| Método | HTTP | Descripción |
|--------|------|-------------|
| `Login()` | GET | Muestra el formulario. Si ya hay sesión, redirige según rol |
| `Login(model)` | POST | Valida credenciales. Si son correctas, guarda sesión y redirige |
| `Logout()` | GET | Limpia la sesión y redirige al login |

La redirección según rol funciona así:
```csharp
if (usuario.Tipo == "Administrador")
    return RedirectToAction("CrearEvento", "Evento");

return RedirectToAction("Listado", "Evento");
```

---

### `Controllers/EventoController.cs`

| Método | HTTP | URL | Acceso |
|--------|------|-----|--------|
| `CrearEvento()` | GET | `/Evento/CrearEvento` | Solo si hay sesión |
| `CrearEvento(evento)` | POST | `/Evento/CrearEvento` | Solo si hay sesión |
| `Listado()` | GET | `/Evento/Listado` | Solo si hay sesión |

**Validaciones del servidor** en `CrearEvento POST`:
- Fecha de inicio no puede ser en el pasado
- Fecha de fin debe ser posterior a la de inicio
- Si el modelo tiene errores (`ModelState.IsValid == false`), regresa la vista con los errores

---

### `Views/Shared/_Layout.cshtml`
Es la **plantilla base** de toda la aplicación. Todas las vistas se insertan dentro de ella en el lugar donde dice `@RenderBody()`. Contiene:

- El `<head>` con los CSS
- La barra de navegación (`<nav>`) que muestra opciones según el rol del usuario
- El contenedor principal con el contenido
- Los scripts de jQuery y Bootstrap al final

---

### `Views/Auth/Login.cshtml`
Usa los componentes de `Shared/` para construir el formulario. Incluye un bloque `@section scripts` con validación JavaScript en tiempo real que marca los campos como inválidos (clase `is-invalid`) al salir de ellos.

---

### `Views/Evento/CrearEvento.cshtml`
Formulario con 4 campos: Nombre, Tipo (combo), Fecha inicio, Fecha fin. La validación JavaScript valida cada campo al perder foco y bloquea el envío si alguno falla. El servidor hace una segunda validación independiente.

---

### `Views/Evento/Listado.cshtml`
Muestra una tabla simple con todos los eventos cuya fecha de inicio sea igual o posterior a `DateTime.Now`, ordenados cronológicamente.

---

### `App_Start/BundleConfig.cs`
Agrupa los archivos CSS y JS en "bundles" para que el navegador los cargue eficientemente:

```csharp
// Todos los CSS
"~/Content/css"  →  bootstrap.css + site.css

// Scripts
"~/bundles/jquery"     →  jquery-3.7.0.js
"~/bundles/bootstrap"  →  bootstrap.bundle.js
```

---

## 🗄 Base de datos

Los scripts están en [`Database/`](SGES.Web/SGES.Web/Database/).

### Diagrama de tablas
<img src="https://github.com/Louis-Du/SGESWeb/blob/develop/modeloRelacionalSGES.png" alt="ModeloRelacional">

### Configurar la BD

1. Abre SQL Server Management Studio (SSMS)
2. Ejecuta [`Database/Shema.sql`](SGES.Web/SGES.Web/Database/Shema.sql) — crea la BD y todas las tablas
3. Ejecuta [`Database/Data.sql`](SGES.Web/SGES.Web/Database/Data.sql) — inserta datos de prueba

**Usuarios de prueba creados:**

| ID | Nombre | Contraseña | Tipo |
|----|--------|-----------|------|
| 1 | Carlos Ruiz | Admin123 | Administrador |
| 2 | Ana Martinez | Admin456 | Administrador |
| 1 (idApr) | Carlos Ramirez | pass123 | Aprendiz |
| 2 (idApr) | Laura Gomez | pass456 | Aprendiz |

> ⚠️ El `idUser` de `Usuario` y el `idApr` de `Aprendiz` son independientes. El login busca primero en `Usuario` y luego en `Aprendiz`.

---

## 🔄 Flujo de una petición de principio a fin

Ejemplo completo: un Administrador crea un evento.

```
1. Usuario escribe /Evento/CrearEvento en el navegador
        ↓
2. RouteConfig.cs lo enruta a EventoController → CrearEvento() [GET]
        ↓
3. El controlador verifica Session["Usuario"] != null
   └─ Si no hay sesión → redirige a /Auth/Login
        ↓
4. Pone los tipos de evento en ViewBag.TiposEvento
        ↓
5. return View(new EventoModel()) → carga CrearEvento.cshtml
        ↓
6. _Layout.cshtml envuelve la vista con el navbar y el footer
        ↓
7. El usuario llena el formulario y hace clic en "Guardar"
        ↓
8. El navegador ejecuta la validación JS (blur/change/submit)
   └─ Si algún campo falla → muestra el error, bloquea el envío
        ↓
9. Si todo OK → POST a /Evento/CrearEvento
        ↓
10. EventoController → CrearEvento(EventoModel evento) [POST]
    ├─ Asigna evento.IdUser desde la sesión
    ├─ Valida fechas en el servidor (ModelState.AddModelError si fallan)
    ├─ if (!ModelState.IsValid) → return View(evento) con errores
    └─ _dao.InsertarEvento(evento) → INSERT en SQL Server
        ↓
11. TempData["Success"] = "Evento creado correctamente."
    → RedirectToAction("CrearEvento")  [GET limpio]
```

---

## 🧱 Componentes visuales reutilizables (Shared/)

> Para la documentación detallada de cada componente, ve a [`Views/Shared/COMPONENTES.md`](SGES.Web/SGES.Web/Views/Shared/COMPONENTES.md).

El proyecto tiene una librería de componentes en `Views/Shared/`. En lugar de escribir HTML repetido en cada vista, se usan **Partial Views** con parámetros vía `ViewData`.

### Patrón de invocación

```cshtml
@Html.Partial("_NombreComponente", null, new ViewDataDictionary(ViewData) {
    { "parametro1", valor1 },
    { "parametro2", valor2 }
})
```

> ⚠️ Siempre usa `new ViewDataDictionary(ViewData)` (no `new ViewDataDictionary()`). Esto preserva los errores de validación del modelo actual.

### Resumen de componentes disponibles

| Componente | Archivo | Para qué sirve |
|-----------|---------|----------------|
| `_TextBox` | [`_TextBox.cshtml`](SGES.Web/SGES.Web/Views/Shared/_TextBox.cshtml) | Campo de texto (text, number, password, date, datetime-local…) |
| `_TextArea` | [`_TextArea.cshtml`](SGES.Web/SGES.Web/Views/Shared/_TextArea.cshtml) | Campo de texto multilínea |
| `_ComboBox` | [`_ComboBox.cshtml`](SGES.Web/SGES.Web/Views/Shared/_ComboBox.cshtml) | Lista desplegable `<select>` |
| `_BotonGuardar` | [`_BotonGuardar.cshtml`](SGES.Web/SGES.Web/Views/Shared/_BotonGuardar.cshtml) | Botón `type="submit"` azul |
| `_BotonCancelar` | [`_BotonCancelar.cshtml`](SGES.Web/SGES.Web/Views/Shared/_BotonCancelar.cshtml) | Enlace de navegación con apariencia de botón |
| `_BotonEliminar` | [`_BotonEliminar.cshtml`](SGES.Web/SGES.Web/Views/Shared/_BotonEliminar.cshtml) | Botón que hace POST con confirmación |
| `_Card` | [`_Card.cshtml`](SGES.Web/SGES.Web/Views/Shared/_Card.cshtml) | Contenedor tipo tarjeta Bootstrap con encabezado |
| `_TablaBasica` | [`_TablaBasica.cshtml`](SGES.Web/SGES.Web/Views/Shared/_TablaBasica.cshtml) | Tabla de datos con soporte para acciones por fila |

---

## ➕ Cómo agregar una nueva funcionalidad

Seguir estos pasos garantiza que la funcionalidad quede integrada correctamente con el resto del proyecto.

### Paso 1 — Crea el Modelo (si hay datos nuevos)

**¿Cuándo?** Si la funcionalidad necesita representar una entidad nueva (ej: `Inscripcion`, `Programa`).

Crea dos archivos en [`Models/`](SGES.Web/SGES.Web/Models/):

**`Models/InscripcionModel.cs`** — la clase que representa los datos:
```csharp
namespace SGES.Web.Models
{
    public class InscripcionModel
    {
        public int IdInscrip { get; set; }
        public DateTime FechaInscrip { get; set; }
        public string Modalidad { get; set; }
        public int IdApr { get; set; }
        public int IdEvento { get; set; }
    }
}
```

**`Models/InscripcionDAO.cs`** — las consultas SQL:
```csharp
namespace SGES.Web.Models
{
    public class InscripcionDAO
    {
        private readonly Conexion cn = new Conexion();

        public void Inscribir(InscripcionModel inscripcion)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"INSERT INTO Inscripciones
                               (fechaInscrip, modalidadInscrip, idApr, idEvento)
                               VALUES (@fecha, @modalidad, @idApr, @idEvento)";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@fecha",      inscripcion.FechaInscrip);
                cmd.Parameters.AddWithValue("@modalidad",  inscripcion.Modalidad);
                cmd.Parameters.AddWithValue("@idApr",      inscripcion.IdApr);
                cmd.Parameters.AddWithValue("@idEvento",   inscripcion.IdEvento);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
```

---

### Paso 2 — Crea o actualiza el Controlador

Si la funcionalidad pertenece a un módulo nuevo, crea un archivo en [`Controllers/`](SGES.Web/SGES.Web/Controllers/).

**`Controllers/InscripcionController.cs`**:
```csharp
using SGES.Web.Models;
using System.Web.Mvc;

namespace SGES.Web.Controllers
{
    public class InscripcionController : Controller
    {
        private readonly InscripcionDAO _dao;

        public InscripcionController() : this(new InscripcionDAO()) { }

        public InscripcionController(InscripcionDAO dao)
        {
            _dao = dao;
        }

        // Acción privada para obtener usuario de sesión
        private UsuarioSesion UsuarioActual
        {
            get { return Session["Usuario"] as UsuarioSesion; }
        }

        [HttpGet]
        public ActionResult Inscribirse(int id)
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            // Aquí podrías cargar datos del evento para mostrarlos
            ViewBag.IdEvento = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Inscribirse(InscripcionModel inscripcion)
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            inscripcion.IdApr = UsuarioActual.Id;
            inscripcion.FechaInscrip = DateTime.Today;

            if (!ModelState.IsValid)
                return View(inscripcion);

            _dao.Inscribir(inscripcion);
            TempData["Success"] = "¡Inscripción realizada con éxito!";
            return RedirectToAction("Listado", "Evento");
        }
    }
}
```

> ⚠️ Siempre verifica `Session["Usuario"] != null` al inicio de cada acción que requiera autenticación.

---

### Paso 3 — Crea la(s) Vista(s)

Crea una carpeta con el nombre del controlador (sin "Controller") dentro de `Views/` y agrega el archivo `.cshtml`.

**`Views/Inscripcion/Inscribirse.cshtml`**:
```cshtml
@model SGES.Web.Models.InscripcionModel

@{
    ViewBag.Title = "Inscribirse al Evento";
}

@* Encabezado con el componente _Card *@
@Html.Partial("_Card", null, new ViewDataDictionary(ViewData) {
    { "titulo",    "Inscripción a Evento" },
    { "icono",     "bi-person-plus" },
    { "subtitulo", "Complete los datos para inscribirse" }
})

@using (Html.BeginForm("Inscribirse", "Inscripcion", FormMethod.Post))
{
    @Html.AntiForgeryToken()

    @* Muestra todos los errores del modelo en un bloque *@
    @Html.ValidationSummary(false, "", new { @class = "alert alert-danger" })

    @Html.Partial("_ComboBox", null, new ViewDataDictionary(ViewData) {
        { "id",            "Modalidad" },
        { "label",         "Modalidad" },
        { "items",         new SelectList(new[] { "Presencial", "Virtual" }) },
        { "selectedValue", Model != null ? Model.Modalidad : "" },
        { "defaultOption", "-- Seleccione --" },
        { "required",      true }
    })
    @Html.ValidationMessage("Modalidad", "", new { @class = "text-danger small" })

    @* Campo oculto para el ID del evento *@
    <input type="hidden" name="IdEvento" value="@ViewBag.IdEvento" />

    <div class="mt-3">
        @Html.Partial("_BotonGuardar", null, new ViewDataDictionary(ViewData) {
            { "texto", "Confirmar inscripción" },
            { "icono", "bi-check-circle" }
        })
        @Html.Partial("_BotonCancelar", null, new ViewDataDictionary(ViewData) {
            { "url", Url.Action("Listado", "Evento") }
        })
    </div>
}

@* Validación JS en tiempo real (opcional pero recomendado) *@
@section scripts {
    <script>
        (function () {
            var modalidad = document.getElementById("Modalidad");

            function getSpan(fieldId) {
                return document.querySelector('[data-valmsg-for="' + fieldId + '"]');
            }

            function validarModalidad() {
                if (!modalidad.value) {
                    modalidad.classList.add("is-invalid");
                    var span = getSpan("Modalidad");
                    if (span) { span.textContent = "Debe seleccionar una modalidad."; }
                    return false;
                }
                modalidad.classList.remove("is-invalid");
                modalidad.classList.add("is-valid");
                return true;
            }

            modalidad.addEventListener("change", validarModalidad);

            document.querySelector("form").addEventListener("submit", function (e) {
                if (!validarModalidad()) e.preventDefault();
            });
        }());
    </script>
}
```

---

### Paso 4 — Agrega el enlace al navbar (si aplica)

Si la nueva página debe aparecer en la barra de navegación, edita [`Views/Shared/_Layout.cshtml`](SGES.Web/SGES.Web/Views/Shared/_Layout.cshtml):

```cshtml
@* Dentro del bloque if (u.Tipo == "Administrador") o fuera según el rol *@
<li>
  @Html.ActionLink("Inscripciones", "Index", "Inscripcion",
      new { area = "" }, new { @class = "nav-link" })
</li>
```

---

### Resumen — archivos que se crean para una nueva funcionalidad

```
✅ Models/NuevaEntidadModel.cs      → Propiedades (qué datos tiene)
✅ Models/NuevaEntidadDAO.cs        → Consultas SQL (cómo se accede a BD)
✅ Controllers/NuevaController.cs   → Lógica de negocio y validaciones
✅ Views/Nueva/
       ├── Index.cshtml             → Listado (si aplica)
       ├── Crear.cshtml             → Formulario de creación (si aplica)
       └── Detalle.cshtml           → Vista detallada (si aplica)

⚙️  Views/Shared/_Layout.cshtml    → Agrega enlace en navbar (si aplica)
```

---

## 🌿 Flujo de trabajo Git (Git Flow)

El proyecto usa **Git Flow** con tres niveles de ramas:

```
main          ← código en producción (estable)
  └── develop ← rama de integración (aquí está este código)
        └── HU-{ID_JIRA}-{nombre-funcionalidad}  ← ramas de trabajo
```

### ⚠️ Regla crítica — nombre de las ramas

Las ramas de funcionalidad **deben empezar con el ID de Jira** porque hay una automatización que vincula los commits con las historias de usuario:

```bash
# ✅ Correcto
git checkout -b HU-14-inscripcion-aprendiz
git checkout -b HU-15-editar-evento
git checkout -b HU-16-eliminar-evento

# ❌ Incorrecto — la automatización no lo detectará
git checkout -b inscripcion-aprendiz
git checkout -b feature/nueva-funcionalidad
```

### Flujo completo para desarrollar una historia de usuario

```bash
# 1. Asegúrate de estar en develop y actualizado
git checkout develop
git pull origin develop

# 2. Crea tu rama (con el ID de Jira)
git checkout -b HU-14-inscripcion-aprendiz

# 3. Desarrolla la funcionalidad...
#    (crea los archivos descritos en la sección anterior)

# 4. Haz commits descriptivos
git add .
git commit -m "HU-14: Agrega modelo y DAO de inscripción"
git commit -m "HU-14: Agrega InscripcionController con validaciones"
git commit -m "HU-14: Agrega vista Inscribirse con componentes Shared"

# 5. Sube tu rama al remoto
git push origin HU-14-inscripcion-aprendiz

# 6. Abre un Pull Request hacia develop en el repositorio remoto
#    (NO mergees directamente a develop sin revisión)
```

### Reglas del proyecto

| Rama | Propósito | ¿Se puede hacer push directo? |
|------|-----------|-------------------------------|
| `main` | Producción | ❌ Solo via PR desde develop |
| `develop` | Integración | ❌ Solo via PR desde rama de HU |
| `HU-{id}-*` | Tu trabajo | ✅ Sí |

---

## ⚙️ Configuración inicial

### Requisitos

- Visual Studio 2022 (o posterior)
- .NET Framework 4.8
- SQL Server (Express funciona)
- SQL Server Management Studio (SSMS)

### Pasos para correr el proyecto localmente

```bash
# 1. Clona el repositorio
git clone <url-del-repositorio>
cd SGESWeb

# 2. Cambia a la rama develop
git checkout develop
```

3. Abre `SGES.Web/SGES.Web.slnx` en Visual Studio

4. Configura la base de datos:
   - Abre SSMS
   - Ejecuta `Database/Shema.sql`
   - Ejecuta `Database/Data.sql`

5. Verifica la cadena de conexión en `Web.config`. Si tu SQL Server local tiene un nombre específico (ej: `DESKTOP-ABC\SQLEXPRESS`), cambia `Data Source=.` por ese nombre:
   ```xml
   connectionString="Data Source=DESKTOP-ABC\SQLEXPRESS;Initial Catalog=SGES;Integrated Security=True;"
   ```

6. Presiona **F5** en Visual Studio para correr el proyecto

7. El navegador abrirá `/Auth/Login` automáticamente

---

<div align="center">
<sub>Rama <code>develop</code> — SGESWeb · SENA</sub>
</div>
