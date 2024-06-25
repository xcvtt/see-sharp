# Неделя 4: домашнее задание

## Перед тем как начать
- Как подготовить окружение [см. тут](./docs/01-prepare-environment.md)
- **САМОЕ ВАЖНОЕ** - полное описание базы данных, схему и описание поле можно найти [тут](./docs/02-db-description.md)
- Воркшоп и примеры запросов [см. тут](./docs/02-db-description.md)

## Основные требования
- решением каждого задания является ОДИН SQL-запрос
- не допускается менять схему или сами данные, если этого явно не указано в задании
- поля в выборках должны иметь псевдоним (alias) указанный в задании
- решение необходимо привести в блоке каждой задачи ВМЕСТО комментария "ЗДЕСЬ ДОЛЖНО БЫТЬ РЕШЕНИЕ" (прямо в текущем readme.md файле)
- метки времени должны быть приведены в формат _dd.MM.yyyy HH:mm:ss_ (время в БД и выборках в UTC)

## Прочие пожелания
- всем будет удобно, если вы будете придерживаться единого стиля форматирования SQL-команд, как в [этом примере](./docs/03-sql-guidelines.md)

## Задание 1: 100 заданий с самым долгим временем выполнения
Время, затраченное на выполнение задания - это период времени, прошедший с момента перехода задания в статус "В работе" и до перехода в статус "Выполнено".
Нужно вывести 100 заданий с самым долгим временем выполнения. 
Полученный список заданий должен быть отсортирован от заданий с наибольшим временем выполнения к заданиям с наименьшим временем выполнения.

Замечания:
- Невыполненные задания (не дошедшие до статуса "Выполнено") не учитываются.
- Когда исполнитель берет задание в работу, оно переходит в статус "В работе" (InProgress) и находится там до завершения работы. После чего переходит в статус "Выполнено" (Done).
  В любой момент времени задание может быть безвозвратно отменено - в этом случае оно перейдет в статус "Отменено" (Canceled).
- Нет разницы выполняется задание или подзадание.
- Выборка должна включать задания за все время.

Выборка должна содержать следующий набор полей:
- номер задания (task_number)
- заголовок задания (task_title)
- название статуса задания (status_name)
- email автора задания (author_email)
- email текущего исполнителя (assignee_email)
- дата и время создания задания (created_at)
- дата и время первого перехода в статус В работе (in_progress_at)
- дата и время выполнения задания (completed_at)
- количество дней, часов, минут и секнуд, которые задание находилось в работе - в формате "dd HH:mm:ss" (work_duration)

### Решение
```sql
   select tl1.task_id as task_number
        , tl1.title as task_title
        , ts.name as status_name
        , u1.email as author_email
        , u2.email as assignee_email
        , t.created_at as created_at
        , tl1.at as in_progres_at
        , t.completed_at as completed_at
        , to_char(tl2.at - tl1.at, 'DD HH24:MI:SS') as work_duration
     from task_logs tl1
     join task_logs tl2 on tl2.task_id = tl1.task_id
     join tasks t on t.id = tl1.task_id
     join task_statuses ts on ts.id = t.status
     join users u1 on u1.id = t.created_by_user_id
     join users u2 on u2.id = t.assigned_to_user_id
    where tl1.status = 3 /* InProgress */
      and tl2.status = 4 /* Done */
      and ts.name != 'Отменено'
    order by work_duration desc
    limit 100;
```

## Задание 2: Выборка для проверки вложенности
Задания могу быть простыми и составными. Составное задание содержит в себе дочерние - так получается иерархия заданий.
Глубина иерархии ограничено Н-уровнями, поэтому перед добавлением подзадачи к текущей задачи нужно понять, может ли пользователь добавить задачу уровнем ниже текущего или нет. Для этого нужно написать выборку для метода проверки перед добавлением подзадания, которая бы вернула уровень вложенности указанного задания и полный путь до него от родительского задания.

Замечания:
- ИД проверяемого задания передаем в sql как параметр _:parent_task_id_
- если задание _Е_ находится на 5м уровне, то путь должен быть "_//A/B/C/D/E_".

Выборка должна содержать:
- только 1 строку
- поле "Уровень задания" (level) - уровень указанного в параметре задания
- поле "Путь" (path)

### Решение
```sql
  with recursive tasks_tree
    as (select t.id
             , t.parent_task_id
             , 1 as level
             , concat('/', t.id::text) as path
          from tasks t
         where t.id = :task_id

         union all

        select t.id
             , t.parent_task_id
             , tt.level + 1 as level
             , concat('/', t.id::text, tt.path) as path
          from tasks t
          join tasks_tree tt on tt.parent_task_id = t.id)
select tr.level as level
     , concat('/', tr.path) as path
  from tasks_tree tr
 order by level desc
 limit 1
```

## Задание 3 (на 10ку): Денормализация
Наш трекер задач пользуется популярностью и количество только активных задач перевалило уже за несколько миллионов. Продакт пришел с очередной юзер-стори:
```
Я как Диспетчер в списке активных задач всегда должен видеть задачу самого высокого уровня из цепочки отдельным полем

Требования:
1. Список активных задач включает в себя задачи со статусом "В работе"
2. Список должен быть отсортирован от самой новой задачи к самой старой
3. В списке должны быть поля:
  - Номер задачи (task_number)
  - Заголовок (task_title)
  - Номер родительской задачи (parent_task_number)
  - Заголовок родительской задачи (parent_task_title)
  - Номер самой первой задачи из цепочки (root_task_number)
  - Заголовок самой первой задачи из цепочки (root_task_title)
  - Email, на кого назначена задача (assigned_to_email)
  - Когда задача была создана (created_at)
 4. Должна быть возможность получить данные с пагинацией по N-строк (@limit, @offset)
```

Обсудив требования с лидом тебе прилетели 2 задачи:
1. Данных очень много и нужно денормализовать таблицу tasks
   Добавить в таблицу tasks поле `root_task_id bigint not null`
   Написать скрипт заполнения нового поля root_task_id для всей таблицы (если задача является рутом, то ее id должен совпадать с root_task_id)
2. Написать запрос получения данных для отображения в списке активных задач
   (!) Выяснилось, что дополнительно еще нужно возвращать идентификаторы задач, чтобы фронтенд мог сделать ссылки на них (т.е. добавить поля task_id, parent_task_id, root_task_id)

<details>
  <summary>FAQ</summary>

**Q: Что такое root_task_id?**

A: Например, есть задача с id=10 и parent_task_id=9, задача с id=9 имеет parent_task_id=8 и т.д. до задача id=1 имеет parent_task_id=null. Для всех этих задач root_task_id=1.

**Q: Не понял в каком формате нужен результат?**

Ожидаемый результат выполнения SQL-запроса:

| task_id | task_number | task_title | parent_task_id | parent_task_number | parent_task_title | root_task_id | root_task_number | root_task_title | assigned_to_email | created_at          |
|---------|-------------|------------|----------------|--------------------|-------------------|--------------|------------------|-----------------|-------------------|---------------------|
| 1       | A123        | Тест 123   | null           | null               | null              | 1            | A123             | Тест 123        | test@test.tt      | 01.01.2023 08:00:00 |
| 2       | B123        | B-тест     | 1              | A123               | Тест 123          | 1            | A123             | Тест 123        | user@test.tt      | 01.01.2023 11:00:00 |
| 3       | C123        | 123-тест   | 2              | B123               | B-тест            | 1            | A123             | Тест 123        | dev@test.tt       | 01.01.2023 11:10:00 |
| 10      | 1-2345      | New task   | null           | null               | null              | 10           | 1-2345           | New task        | test@test.tt      | 12.02.2024 11:00:00 |

**Q: Все это можно делать в одной миграции?**

А: Нет, каждая DDL операция - отдельная миграция, DML-операция тоже долзна быть в отдельной миграции.

</details>

### Скрипты миграций
```sql
alter table tasks
    add column if not exists root_task_id bigint null;

begin;

  with recursive tasks_tree
    as (select t.id
             , t.parent_task_id
             , t.id as original_id
          from tasks t

         union all

        select t.id
             , t.parent_task_id
             , tt.original_id as original_id
          from tasks t
          join tasks_tree tt on tt.parent_task_id = t.id),

final as (select id as root_task_id
               , original_id as task_id
            from tasks_tree
           where parent_task_id is null)
update tasks t2
   set root_task_id = f.root_task_id
  from final as f
 where f.task_id = t2.id;

commit;

alter table tasks
    alter column root_task_id set not null;
```

### Запрос выборки
```sql
   select t1.id as task_id
        , t1.number as task_number
        , t1.title as task_title
        , t1.parent_task_id as parent_task_id
        , t2.number as parent_task_number
        , t2.title as parent_task_title
        , t1.root_task_id as root_task_id
        , t3.number as root_task_number
        , t3.title as root_task_title
        , u.email as assigned_to_email
        , t1.created_at as created_at
     from tasks t1
left join tasks t2 on t2.id = t1.parent_task_id
     join tasks t3 on t3.id = t1.root_task_id
     join users u on u.id = t1.assigned_to_user_id
    where t1.status = 3 /* InProgress */
    order by t1.id desc
    limit :page_size
   offset (:page_number - 1) * :page_size
```
