using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AmoDownloaderCLR.Models
{
    public class TaskResponse
    {
        [JsonProperty("tasks")]
        public TasksResponse tasks { get; set; }

        [JsonProperty("server_time")]
        public int Amo_time { get; set; }

        public int program_time { get; set; }
    }

    public class TaskResponseRoot
    {
        [JsonProperty("response")]
        public TaskResponse response { get; set; }
    }

    public class TaskAdd
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("element_id")]
        public Int32 element_id { get; set; } //Уникальный идентификатор контакта или сделки (сделка или контакт указывается в element_type)

        [JsonProperty("element_type")]
        public int element_type { get; set; } /*Тип привязываемого елемента (1 - контакт, 2- сделка, 3 - компания)*/

        [JsonProperty("task_type")]
        public int task_type { get; set; } /*Тип задачи (типы задач см. Информация аккаунта)*/

        [JsonProperty("status")]
        public int status { get; set; }  /*Статус задачи (0 - не завершена, 1 - завершена)*/

        [JsonProperty("text")]
        public string text { get; set; } /*Текст задачи*/

        [JsonProperty("complete_till")]
        public Int32 complete_till { get; set; }  //Дата до которой необходимо завершить задачу. Если указано время 23:59, то в интерфейсах системы вместо времени будет отображаться "Весь день"

        [JsonProperty("date_create")]
        public int? date_create { get; set; } //Дата создания данной задачи (не обязательный параметр)

        [JsonProperty("last_modified")]
        public int? last_modified { get; set; } //Дата последнего изменения данной задачи (не обязательный параметр)

        [JsonProperty("request_id")]
        public int? request_id { get; set; } //Внешний идентификатор записи (не обязательный параметр)(Информация о request_id нигде не сохраняется, лишь передается обратно в ответе)

        [JsonProperty("responsible_user_id")]
        public int? responsible_user_id { get; set; } //Уникальный идентификатор ответственного пользователя(пользователи см. Информация аккаунта)
    }

    public class TaskUpdate
    {
        [JsonProperty("id")]
        public int id { get; set; } //Уникальный идентификатор обновляемой задачи

        [JsonProperty("request_id")]
        public int? request_id { get; set; }

        [JsonProperty("element_id")]
        public Int32 element_id { get; set; } //Уникальный идентификатор контакта или сделки (сделка или контакт указывается в element_type)

        [JsonProperty("element_type")]
        public int element_type { get; set; }  /*Тип привязываемого елемента (1 - контакт, 2- сделка, 3 - компания)*/

        [JsonProperty("last_modified")]
        public int last_modified { get; set; } //Дата последнего изменения данной сущности, если параметр не указан, или он меньше чем имеющийся в БД, 
                                                //то обновление не произойдет и в ответ придет информация из Базы Данных amoCRM (Является обязательным параметром)
        [JsonProperty("task_type")]
        public int task_type { get; set; }

        [JsonProperty("text")]
        public string text { get; set; } //Текст задачи

        [JsonProperty("complete_till")]
        public Int32 complete_till { get; set; }

        [JsonProperty("status")]
        public int status { get; set; }

        [JsonProperty("group_id")]
        public int group_id { get; set; }

        [JsonProperty("account_id")]
        public Int32 account_id { get; set; }

        [JsonProperty("result")]
        public List<object> result { get; set; }

    }

    public class TaskUpdateResponse
    {
        [JsonProperty("id")]
        public int id { get; set; } //Уникальный идентификатор обновляемой задачи

        [JsonProperty("request_id")]
        public int? request_id { get; set; }

        [JsonProperty("element_id")]
        public Int32 element_id { get; set; } //Уникальный идентификатор контакта или сделки (сделка или контакт указывается в element_type)

        [JsonProperty("element_type")]
        public int element_type { get; set; }  /*Тип привязываемого елемента (1 - контакт, 2- сделка, 3 - компания)*/

        [JsonProperty("last_modified")]
        public int last_modified { get; set; } //Дата последнего изменения данной сущности, если параметр не указан, или он меньше чем имеющийся в БД, 
                                               //то обновление не произойдет и в ответ придет информация из Базы Данных amoCRM (Является обязательным параметром)
        [JsonProperty("task_type")]
        public string task_type { get; set; }

        [JsonProperty("text")]
        public string text { get; set; } //Текст задачи

        [JsonProperty("complete_till")]
        public Int32 complete_till { get; set; }

        [JsonProperty("status")]
        public int status { get; set; }

        [JsonProperty("group_id")]
        public int group_id { get; set; }

        [JsonProperty("account_id")]
        public Int32 account_id { get; set; }

        [JsonProperty("result")]
        public List<object> result { get; set; }

    }

    public class TasksResponse
    {
        [JsonProperty("add")]
        public List<TaskAdd> add { get; set; }

        [JsonProperty("update")]
        public List<TaskUpdateResponse> update { get; set; }
    }

    public class Tasks
    {
        [JsonProperty("add")]
        public List<TaskAdd> add { get; set; }

        [JsonProperty("update")]
        public List<TaskUpdate> update { get; set; }
    }

    public class Request
    {
        [JsonProperty("tasks")]
        public Tasks tasks { get; set; }
    }

    public class Task
    {
        [JsonProperty("request")]
        public Request request { get; set; }
    }







}
