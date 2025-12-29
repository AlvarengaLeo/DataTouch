# 🤖 Instrucciones para Agente LLM - DataTouch

Este archivo contiene prompts optimizados para que un agente LLM con capacidad de ejecución de comandos (como Claude, Gemini con herramientas, o GitHub Copilot) pueda clonar y ejecutar el proyecto automáticamente.

---

## ⭐ Prompt 0: Auto-Setup Leyendo Documentación (RECOMENDADO)

Este es el prompt más completo y autónomo. El agente leerá la documentación del proyecto y ejecutará los pasos automáticamente:

```
Clona el repositorio https://github.com/AlvarengaLeo/DataTouch.git y configúralo siguiendo la documentación oficial del proyecto.

Instrucciones:
1. Clona el repositorio en una carpeta local
2. Lee el archivo README.md para entender la estructura del proyecto
3. Lee el archivo SETUP.md y sigue TODOS los pasos de instalación que describe
4. Verifica los prerrequisitos (.NET 9, Docker si aplica)
5. Instala las dependencias necesarias
6. Ejecuta la aplicación
7. Verifica que funciona accediendo a las URLs indicadas en la documentación
8. Si encuentras errores, consulta la sección de Troubleshooting en SETUP.md

Credenciales de demo: admin@demo.com / admin123

Ejecuta cada paso y confírmame cuando la aplicación esté funcionando correctamente.
```

---

## ⭐ Prompt 0.1: Auto-Setup con Base de Datos

Si necesitas configurar MySQL además de la aplicación:

```
Clona el repositorio https://github.com/AlvarengaLeo/DataTouch.git y configúralo con MySQL.

Instrucciones:
1. Clona el repositorio
2. Lee el archivo SETUP.md - sección "Configuración de Base de Datos"
3. Lee el archivo DATABASE.md para entender el esquema de tablas
4. Configura MySQL siguiendo los pasos de la documentación (usando Docker)
5. Modifica el Program.cs según indica SETUP.md para usar MySQL en lugar de InMemory
6. Ejecuta los scripts SQL de DATABASE.md si es necesario
7. Ejecuta la aplicación y verifica la conexión a la base de datos

Si hay errores de conexión, revisa la sección de troubleshooting.
```

---

## Prompt 1: Clonar y Ejecutar (Básico)

Copia y pega este prompt al agente:

```
Clona el repositorio https://github.com/AlvarengaLeo/DataTouch.git y ejecuta la aplicación siguiendo estos pasos:

1. Clona el repositorio en una carpeta local
2. Restaura las dependencias con `dotnet restore`
3. Navega a `src/DataTouch.Web`
4. Ejecuta la aplicación con `dotnet run`
5. Confirma que la aplicación está corriendo y muéstrame las URLs de acceso

Credenciales de demo:
- Email: admin@demo.com
- Password: admin123
```

---

## Prompt 2: Clonar, Configurar y Ejecutar (Completo)

Para un setup más detallado con verificaciones:

```
Necesito que configures y ejecutes el proyecto DataTouch en mi máquina. Sigue estos pasos:

## Paso 1: Verificar Prerrequisitos
- Verifica que .NET 9 SDK está instalado (`dotnet --version`)
- Verifica que Git está instalado (`git --version`)

## Paso 2: Clonar el Repositorio
git clone https://github.com/AlvarengaLeo/DataTouch.git
cd DataTouch

## Paso 3: Restaurar Dependencias
dotnet restore

## Paso 4: Compilar el Proyecto
dotnet build

## Paso 5: Ejecutar la Aplicación
cd src/DataTouch.Web
dotnet run

## Paso 6: Verificar que funciona
La aplicación debería estar disponible en:
- https://localhost:5001/login (Panel CRM)
- https://localhost:5001/p/demo-company/admin-demo (Tarjeta pública)

Credenciales: admin@demo.com / admin123

Si hay algún error, revisa el archivo SETUP.md en el repositorio para troubleshooting.
```

---

## Prompt 3: Configuración con MySQL (Producción)

Para usar MySQL en lugar de InMemory:

```
Configura el proyecto DataTouch con MySQL usando Docker:

## Paso 1: Clonar
git clone https://github.com/AlvarengaLeo/DataTouch.git
cd DataTouch

## Paso 2: Iniciar MySQL con Docker
docker run --name datatouch-mysql -e MYSQL_ROOT_PASSWORD=datatouch123 -e MYSQL_DATABASE=datatouch -p 3306:3306 -d mysql:8

## Paso 3: Modificar Program.cs
En el archivo `src/DataTouch.Web/Program.cs`, cambia las líneas 22-24 de:
```csharp
builder.Services.AddDbContext<DataTouchDbContext>(options =>
    options.UseInMemoryDatabase("DataTouchDb"));
```
A:
```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataTouchDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
```

## Paso 4: Restaurar, compilar y ejecutar
dotnet restore
dotnet build
cd src/DataTouch.Web
dotnet run

Verifica que la aplicación conecta correctamente a MySQL.
```

---

## Prompt 4: Para Desarrollo Continuo

Si ya tienes el proyecto clonado y quieres seguir desarrollando:

```
Tengo el proyecto DataTouch clonado en [RUTA DEL PROYECTO].

Por favor:
1. Ve a esa carpeta
2. Verifica el estado de Git (`git status`)
3. Haz pull de los últimos cambios (`git pull`)
4. Ejecuta la aplicación con hot reload (`dotnet watch run` en src/DataTouch.Web)
5. Abre el navegador en https://localhost:5001/login
```

---

## Prompt 5: Crear Nuevo Feature

Para pedirle al agente que agregue funcionalidad:

```
En el proyecto DataTouch (https://github.com/AlvarengaLeo/DataTouch.git):

1. Clona el repositorio si no lo tienes
2. Crea una nueva rama: `git checkout -b feature/[nombre-feature]`
3. [DESCRIBE LA FUNCIONALIDAD A AGREGAR]
4. Compila y verifica que no hay errores
5. Ejecuta la aplicación para verificar visualmente
6. Haz commit de los cambios con un mensaje descriptivo
```

---

## 📋 Variables a Personalizar

Cuando uses estos prompts, reemplaza estas variables según tu entorno:

| Variable | Descripción | Ejemplo |
|----------|-------------|---------|
| `[RUTA DEL PROYECTO]` | Carpeta donde clonar | `C:\Proyectos\DataTouch` |
| `[nombre-feature]` | Nombre de la feature branch | `add-export-pdf` |
| Puerto | Si 5001 está ocupado | Cambiar en launchSettings.json |

---

## 🔧 Comandos de Referencia Rápida

```bash
# Clonar
git clone https://github.com/AlvarengaLeo/DataTouch.git

# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Ejecutar (desarrollo)
cd src/DataTouch.Web
dotnet run

# Ejecutar con hot reload
dotnet watch run

# Ejecutar tests
dotnet test

# Ver logs detallados
dotnet run --verbosity detailed
```

---

## ⚠️ Troubleshooting Común

### Error: Puerto 5001 en uso
```bash
# Cambiar puerto en Properties/launchSettings.json
# O matar el proceso que usa el puerto
netstat -ano | findstr :5001
taskkill /PID [PID] /F
```

### Error: .NET 9 no encontrado
```bash
# Instalar .NET 9
winget install Microsoft.DotNet.SDK.9
# Reiniciar terminal después de instalar
```

### Error: SSL Certificate
```bash
dotnet dev-certs https --trust
```

---

## 📚 Documentación Adicional

- **[SETUP.md](./SETUP.md)** - Guía completa de instalación
- **[DATABASE.md](./DATABASE.md)** - Esquema de base de datos
- **[README.md](./README.md)** - Documentación principal

---

*Instrucciones para agentes LLM - DataTouch MVP 0.1*
