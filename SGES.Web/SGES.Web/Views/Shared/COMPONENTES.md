# SGESWeb — Componentes Shared (Views/Shared/)

Guía de uso de los Partial Views reutilizables.  
Todos los componentes reciben parámetros vía **ViewData** y son exclusivamente de **presentación**.

---

## Cómo invocar un Partial View con parámetros

```cshtml
@Html.Partial("_NombreComponente", null, new ViewDataDictionary(ViewData) {
    { "parametro1", valor1 },
    { "parametro2", valor2 }
})
```

> Siempre usar `new ViewDataDictionary(ViewData)` para preservar los errores de validación del modelo actual.

---

## _TextBox

Campo de texto de una línea.

```cshtml
@Html.Partial("_TextBox", null, new ViewDataDictionary(ViewData) {
    { "id",          "nombre" },
    { "label",       "Nombre del Estudiante" },
    { "value",       Model.Nombre },
    { "placeholder", "Ingrese el nombre completo..." },
    { "required",    true },
    { "maxLength",   100 }
})
```

**Parámetros clave:** `id`, `label`, `value`, `placeholder`, `type`, `required`, `disabled`, `readonly`, `maxLength`, `helpText`, `cssClass`

---

## _TextArea

Campo de texto multilínea.

```cshtml
@Html.Partial("_TextArea", null, new ViewDataDictionary(ViewData) {
    { "id",          "observaciones" },
    { "label",       "Observaciones" },
    { "value",       Model.Observaciones },
    { "rows",        5 },
    { "placeholder", "Ingrese observaciones relevantes..." },
    { "helpText",    "Máximo 500 caracteres." },
    { "maxLength",   500 }
})
```

**Parámetros clave:** `id`, `label`, `value`, `rows`, `placeholder`, `required`, `maxLength`, `resize`, `helpText`

---

## _ComboBox

Lista desplegable (select).

```cshtml
@Html.Partial("_ComboBox", null, new ViewDataDictionary(ViewData) {
    { "id",            "gradoId" },
    { "label",         "Grado" },
    { "items",         new SelectList(ViewBag.Grados, "GradoId", "Nombre") },
    { "selectedValue", Model.GradoId },
    { "defaultOption", "-- Seleccione un grado --" },
    { "required",      true }
})
```

**Parámetros clave:** `id`, `label`, `items` (SelectList), `selectedValue`, `defaultOption`, `required`, `disabled`, `onchange`

---

## _BotonGuardar

Botón `type="submit"` para formularios.

```cshtml
@Html.Partial("_BotonGuardar")

@* Personalizado *@
@Html.Partial("_BotonGuardar", null, new ViewDataDictionary(ViewData) {
    { "texto", "Registrar Estudiante" },
    { "icono", "bi-person-plus-fill" }
})
```

**Parámetros clave:** `texto`, `icono`, `disabled`, `cssClass`, `id`, `form`

---

## _BotonCancelar

Enlace de navegación con apariencia de botón.

```cshtml
@Html.Partial("_BotonCancelar", null, new ViewDataDictionary(ViewData) {
    { "url",     Url.Action("Index", "Estudiantes") },
    { "texto",   "Volver al listado" },
    { "confirm", "¿Desea cancelar? Los cambios no se guardarán." }
})
```

**Parámetros clave:** `url`, `texto`, `icono`, `confirm`, `disabled`, `cssClass`

---

## _BotonEliminar

Formulario POST con confirmación de seguridad.

```cshtml
@* En una tabla — botón compacto *@
@Html.Partial("_BotonEliminar", null, new ViewDataDictionary(ViewData) {
    { "id",          item.EstudianteId },
    { "controlador", "Estudiantes" },
    { "accion",      "Delete" },
    { "size",        "sm" },
    { "outline",     true }
})
```

**Parámetros clave:** `id`, `controlador`, `accion`, `area`, `texto`, `confirm`, `size`, `outline`, `disabled`

---

## _TablaSeleccion

Tabla de datos con encabezados y columnas de acción opcionales.

```cshtml
@Html.Partial("_TablaSeleccion", null, new ViewDataDictionary(ViewData) {
    { "titulo",   "Listado de Estudiantes" },
    { "columnas", new[] { "Código", "Nombre", "Apellido", "Grado" } },
    { "filas",    Model.Select(e => new object[] {
                      e.Codigo, e.Nombre, e.Apellido, e.Grado.Nombre }) },
    { "acciones", new[] { "Acciones" } },
    { "accionesHtml", Model.Select(e => new[] {
          Html.Partial("_BotonEliminar", null, new ViewDataDictionary(ViewData) {
              { "id", e.EstudianteId }, { "controlador", "Estudiantes" }, { "size", "sm" }
          }) }) }
})
```

**Parámetros clave:** `columnas`, `filas`, `acciones`, `accionesHtml`, `titulo`, `striped`, `hover`, `small`, `responsive`, `mensajeVacio`

---

## _Card

Contenedor de tarjeta con encabezado y pie opcionales.

```cshtml
@{
    var cuerpo = new System.Web.Mvc.HtmlHelper(...); // HTML pre-renderizado
}

@Html.Partial("_Card", null, new ViewDataDictionary(ViewData) {
    { "titulo",     "Datos del Estudiante" },
    { "subtitulo",  "Complete todos los campos obligatorios" },
    { "icono",      "bi-person-vcard" },
    { "shadow",     "sm" },
    { "cuerpoHtml", cuerpoHtml }
})
```

**Parámetros clave:** `titulo`, `subtitulo`, `icono`, `footer`, `cuerpoHtml`, `shadow`, `color`, `sinBorde`, `cssClass`

---

## Ejemplo: Vista completa (Create.cshtml)

```cshtml
@model SGESWeb.Models.EstudianteViewModel

@{
    ViewBag.Title = "Registrar Estudiante";
}

@Html.Partial("_Card", null, new ViewDataDictionary(ViewData) {
    { "titulo",    "Nuevo Estudiante" },
    { "icono",     "bi-person-plus" },
    { "subtitulo", "Complete los datos del nuevo estudiante" }
})

@using (Html.BeginForm("Create", "Estudiantes", FormMethod.Post))
{
    @Html.AntiForgeryToken()

    <div class="row g-3">
        <div class="col-md-6">
            @Html.Partial("_TextBox", null, new ViewDataDictionary(ViewData) {
                { "id", "Nombre" }, { "label", "Nombre" },
                { "value", Model.Nombre }, { "required", true }
            })
        </div>
        <div class="col-md-6">
            @Html.Partial("_TextBox", null, new ViewDataDictionary(ViewData) {
                { "id", "Apellido" }, { "label", "Apellido" },
                { "value", Model.Apellido }, { "required", true }
            })
        </div>
        <div class="col-md-4">
            @Html.Partial("_ComboBox", null, new ViewDataDictionary(ViewData) {
                { "id", "GradoId" }, { "label", "Grado" },
                { "items", new SelectList(ViewBag.Grados, "GradoId", "Nombre") },
                { "selectedValue", Model.GradoId },
                { "defaultOption", "-- Seleccione --" },
                { "required", true }
            })
        </div>
        <div class="col-12">
            @Html.Partial("_TextArea", null, new ViewDataDictionary(ViewData) {
                { "id", "Observaciones" }, { "label", "Observaciones" },
                { "value", Model.Observaciones }, { "rows", 3 }
            })
        </div>
    </div>

    <div class="sges-acciones-form">
        @Html.Partial("_BotonGuardar")
        @Html.Partial("_BotonCancelar", null, new ViewDataDictionary(ViewData) {
            { "url", Url.Action("Index", "Estudiantes") }
        })
    </div>
}
```

---

## Estructura de archivos

```
SGESWeb/
├── Views/
│   └── Shared/
│       ├── _TextBox.cshtml
│       ├── _TextArea.cshtml
│       ├── _ComboBox.cshtml
│       ├── _BotonGuardar.cshtml
│       ├── _BotonCancelar.cshtml
│       ├── _BotonEliminar.cshtml
│       ├── _TablaSeleccion.cshtml
│       └── _Card.cshtml
└── wwwroot/
    └── css/
        └── site.css
```
