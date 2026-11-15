# Практичне заняття 4. Виведення інформації у форматі списку (.NET MAUI)

Мобільний застосунок-каталог фільмів на базі **.NET MAUI** із завантаженням даних із публічного REST API `https://api.imdbapi.dev/`.

## Реалізований функціонал
- **Робота з REST API та DI:** клієнт `HttpClient` та сервіс `ImdbService` зареєстровані в DI-контейнері (`MauiProgram.cs`).
- **Модель та MVVM:** створено модель `MovieModel` та `MainPageModel` на базі `CommunityToolkit.Mvvm`.
- **Список і пагінація:** використано `CollectionView` із `ItemTemplate`, `UriImageSource` (із кешуванням постерів) та автоматичним довантаженням наступних сторінок при прокручуванні вниз (`RemainingItemsThreshold` / `RemainingItemsThresholdReachedCommand`).
- **Адаптивність:** автоматична перебудова сітки елементів (`GridItemsLayout` зі зміною `Span` між 1 та 2 колонками) при зміні орієнтації екрана (вертикальна / горизонтальна).
