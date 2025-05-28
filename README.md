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
