# Events

## 1. Клонируйте репозиторий

```sh
git clone https://github.com/stsvch/Events.git
cd Events
```

## 2. Запустите базу данных

```sh
docker-compose up db
```

Дождитесь, пока база данных полностью инициализируется!!!

## 3. Запустите backend


```sh
docker-compose up backend
```

Если при первом запуске возникла ошибка подключения — дождитесь, пока контейнер с БД завершит инициализацию, затем перезапустите backend:

```sh
docker-compose restart backend
```

## 4. Запустите frontend

```sh
docker-compose up frontend
```

## 5. Доступ к сервисам

* **Backend:** [https://localhost:7188/](https://localhost:7188/)
* **Frontend:** [http://localhost:3000/](http://localhost:3000/)

## 6. Остановить все контейнеры

```sh
docker-compose down
```

---

## Данные для входа администратора

* **Логин:** `admin`
* **Пароль:** `Admin1`

---
## Регистрация и авторизация пользователей

* Для регистрации нового пользователя необходимо указать **уникальные** логин и электронную почту.
* Пароль должен содержать **минимум 6 символов** и обязательно включать **буквы в верхнем и нижнем регистрах**, а также **цифры**.
* После успешной регистрации выполните вход в свой аккаунт для доступа к регистрации на события и другим функциям.
* **Без авторизации** доступ к регистрации на события и другим закрытым разделам недоступен.

