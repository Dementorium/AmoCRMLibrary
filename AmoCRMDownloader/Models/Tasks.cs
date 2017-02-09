using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmoCRMDownloader.Models
{

    public class Add
    {
        public int id { get; set; }
        public int element_id { get; set; } //Уникальный идентификатор контакта или сделки (сделка или контакт указывается в element_type)
        public int element_type { get; set; } /*Тип привязываемого елемента (1 - контакт, 2- сделка, 3 - компания)*/
        public int task_type { get; set; } /*Тип задачи (типы задач см. Информация аккаунта)*/
        public int status { get; set; } /*Статус задачи (0 - не завершена, 1 - завершена)*/
        public string text { get; set; } /*Текст задачи*/
        public int complete_till { get; set; } //Дата до которой необходимо завершить задачу. Если указано время 23:59, то в интерфейсах системы вместо времени будет отображаться "Весь день"
        public int? date_create { get; set; } //Дата создания данной задачи (не обязательный параметр)
        public int? last_modified { get; set; } //Дата последнего изменения данной задачи (не обязательный параметр)
        public int? request_id { get; set; } //Внешний идентификатор записи (не обязательный параметр)(Информация о request_id нигде не сохраняется, лишь передается обратно в ответе)
        public int? responsible_user_id { get; set; } //Уникальный идентификатор ответственного пользователя(пользователи см. Информация аккаунта)
    }

    public class Update
    {
        public int id { get; set; } //Уникальный идентификатор обновляемой задачи
        public int request_id { get; set; }
        public int element_id { get; set; } //Уникальный идентификатор контакта или сделки (сделка или контакт указывается в element_type)
        public int element_type { get; set; } /*Тип привязываемого елемента (1 - контакт, 2- сделка, 3 - компания)*/
        public int last_modified { get; set; } //Дата последнего изменения данной сущности, если параметр не указан, или он меньше чем имеющийся в БД, 
                                                //то обновление не произойдет и в ответ придет информация из Базы Данных amoCRM (Является обязательным параметром)
        public int task_type { get; set; }
        public string text { get; set; } //Текст задачи
        public int complete_till { get; set; } 
    }

    public class Tasks
    {
        public List<Add> add { get; set; }
        public List<Update> update { get; set; }
        public int server_time { get; set; }
    }

    public class Request
    {
        public Tasks tasks { get; set; }
    }

    public class Task
    {
        public Request request { get; set; }
    }

}
