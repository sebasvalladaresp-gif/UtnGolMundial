# UTN GolMundial 2026 — Proyecto .NET (sin base de datos, todo en memoria)

Un solo proyecto ASP.NET Core (MVC + APIs) que junta lo que sería el backend
UTNGolCoin y el de Estadísticas/Partidos, con vistas Razor ya conectadas.
No necesita base de datos: los datos viven en memoria mientras el proceso está
corriendo (si haces `Ctrl+C` y vuelves a correr, se reinician los datos —
incluyendo el usuario admin de prueba y los partidos semilla).

## PASO 1 — Instalar el SDK de .NET 8

1. Ve a: https://dotnet.microsoft.com/download/dotnet/8.0
2. Descarga el **SDK de .NET 8.0** (no el "Runtime", el **SDK** completo) para tu sistema operativo.
3. Instálalo (siguiente, siguiente, siguiente).
4. Verifica que quedó instalado abriendo una terminal (CMD/PowerShell) y escribiendo:
   ```
   dotnet --version
   ```
   Debe mostrarte algo como `8.0.4xx`. Si te da "no se reconoce como comando", reinicia la terminal (o el computador) y vuelve a intentar — a veces el PATH no se actualiza hasta reiniciar.

## PASO 2 — Descomprimir el proyecto

Descomprime el zip en una carpeta fácil de encontrar, por ejemplo `C:\proyectos\UtnGolMundial`.

## PASO 3 — Correr el proyecto

Abre una terminal **dentro de la carpeta `UtnGolMundial.Web`** (la que tiene el archivo `.csproj`) y ejecuta:

```bash
dotnet run
```

La primera vez va a descargar y compilar cosas, tarda un poco. Cuando veas algo como:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

...ya está corriendo. Abre el navegador en **http://localhost:5000**

Debería abrirse solo (el proyecto está configurado para abrir el navegador automáticamente).

## PASO 4 — Probar la aplicación

### Como usuario normal
1. Click en "Registrarse", crea una cuenta — te acredita 10 UTNGolCoin automáticamente.
2. En el calendario, elige un partido que aún no haya iniciado, pon un resultado y un monto, dale "Predecir".
3. Ve a "Mi Billetera" para ver el historial de esa transacción (el descuento del monto apostado).

### Como administrador (para probar la liquidación de premios)
1. Cierra sesión, inicia sesión con: **admin@utn.edu.ec / admin123**
2. Ve a "Admin: Partidos".
3. Busca el partido que tiene una predicción pendiente (o crea uno nuevo y espera a que pase la hora, o edita la hora del partido semilla para el pasado) y regístrale un resultado.
4. Esto liquida automáticamente todas las predicciones pendientes de ese partido — vuelve a entrar con el usuario que predijo y revisa su saldo/billetera: si acertó, debería haber subido.

## Si `dotnet run` te tira un error

Copia el mensaje de error completo (usualmente dice algo como "CS0103" o similar con el archivo y línea) y mándamelo tal cual — lo corregimos de inmediato. No debería tomar más de un par de minutos arreglarlo.

## Endpoints de API (además de las vistas)

| Método | Ruta                        | Descripción                          | Auth |
|--------|-----------------------------|----------------------------------------|------|
| POST   | /api/auth/registro           | Registro (también inicia sesión)       | No |
| POST   | /api/auth/login              | Login                                  | No |
| GET    | /api/billetera/saldo         | Saldo actual                           | Sí |
| GET    | /api/billetera/historial     | Historial                              | Sí |
| GET    | /api/partidos                | Listar partidos                        | No |
| PUT    | /api/partidos/{id}/resultado | Registrar resultado (dispara liquidación) | Admin |
| POST   | /api/predicciones            | Crear predicción                       | Sí |
| GET    | /api/predicciones/mias       | Mis predicciones                       | Sí |
| GET    | /api/ranking                 | Ranking público                        | No |

La autenticación de la API usa la misma cookie que las vistas — si te logueaste
en el navegador, esas rutas ya funcionan (por ejemplo pega `http://localhost:5000/api/billetera/saldo`
en una pestaña nueva del mismo navegador después de loguearte).

## Estructura del proyecto

```
UtnGolMundial.Web/
├── Models/           -> Usuario, Billetera, Transaccion, Partido, Prediccion
├── Data/              -> InMemoryStore.cs (reemplaza a la base de datos por ahora)
├── Services/          -> lógica de negocio (AuthService, BilleteraService, PartidoService, PrediccionService)
├── Controllers/        -> controladores MVC (vistas)
├── Controllers/Api/    -> controladores de API (JSON)
└── Views/              -> las páginas Razor (.cshtml)
```

## Cuando quieras conectar una base de datos real

`InMemoryStore.cs` es la única pieza que tendría que cambiar de raíz (pasar a
Entity Framework Core + PostgreSQL o SQL Server). Los Services y Controllers
no deberían necesitar cambios grandes porque ya están escritos contra una capa
de datos separada.
