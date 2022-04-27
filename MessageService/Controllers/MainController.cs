using System;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessageService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessageService.Controllers
{
    /// <summary>
    /// Основной контроллер.
    /// </summary>
    [Route("/[controller]")]
    public class MainController : Controller
    {
        // Список пользователей.
        private static List<User> _users = new List<User>();

        // Отсортированный список пользователей.
        public List<User> SortedUsers => _users.OrderBy(x => x.Email).ToList();

        // Список писем.
        private static List<Mail> _mails = new List<Mail>();

        // Список возможных сообщений для случайной генерации.
        private static string[] _messages = new[]
        {
            "Hi!", "Hello", "How are you?", "testing", "hse is best",
            "Wanna anekdot?", "Итак, однажды в бар заходит...", "Выыхооодииилаааааа на берег катюша",
            "Это сообщение сгенерировано", "Данное письмо сгенерировано автоматически, отвечать на него не нужно.",
            "Поздравляем! У вас 0 за пир!", "Джейк, нужно срочно поговорить", "Хьюстон, у нас проблемы",
            "Это и то легче фракталов", "Джонни, пора убираться отсюда", "Ayo!", "heh", "Связь прерыва...*пжджздж*",
            "а можно кр по дискре онлайн?"
        };

        // Список возможных имён для случайной генерации.
        private static string[] _userNames = new[]
        {
            "Джони", "Вован", "Петя", "Мирный житель", "Джонни Уикки",
            "Баба яга", "Вован v2.0", "Засмотревшийся на закат", "Уходящий в закат",
            "Дейнерис матерь драконов Таргариен", "Четырехлистный клевер",
            "не мирный житель", "Стив", "Бонд. Джеймс Бонд."
        };

        // Список возможных почтовых доменов для случайно генерации.
        private static string[] _mailDomains = new[]
        {
            "@gmail.com", "@yandex.ru",
            "@mail.ru", "@outlook.com", "@edu.hse.ru"
        };

        private static Random _random = new Random();

        /// <summary>
        /// Генерация случайного почтового адреса.
        /// </summary>
        /// <param name="users">Список пользователей для проверки не существует ли уже сгенерированного адреса.</param>
        /// <returns>Случайный почтовый адрес.</returns>
        private static string GenerateEmail(List<User> users)
        {
            string allowedSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvxyz";

            string result = String.Empty;

            int nameLength = _random.Next(4, 10);

            for (int i = 0; i < nameLength; i++)
            {
                result += allowedSymbols[_random.Next(allowedSymbols.Length - 1)];
            }

            result += _mailDomains[_random.Next(_mailDomains.Length - 1)];

            // Проверка на существование данного адреса.
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Email == result) return GenerateEmail(users);
            }

            return result;
        }

        /// <summary>
        /// Случайное заполнение списков.
        /// </summary>
        /// <returns>Код 200</returns>
        [HttpPost]
        public IActionResult ListsInitializer()
        {
            int mailNumber = _random.Next(3, 11);

            _users = new List<User>(mailNumber * 2);
            _mails = new List<Mail>(mailNumber);

            for (int i = 0; i < _users.Capacity; i++)
            {
                User tempUser = new User
                {
                    UserName = _userNames[_random.Next(_userNames.Length - 1)],
                    Email = GenerateEmail(_users)
                };

                _users.Add(tempUser);
            }

            for (int i = 0; i < _mails.Capacity; i++)
            {
                Mail tempMail = new Mail
                {
                    Message = _messages[_random.Next(_messages.Length - 1)],
                    ReceiverId = _users[_random.Next(_users.Count - 1)].Email,
                    SenderId = _users[_random.Next(_users.Count - 1)].Email,
                    Subject = _messages[_random.Next(_messages.Length - 1)]
                };

                _mails.Add(tempMail);
            }

            return Ok();
        }

        /// <summary>
        /// Json cериализация списка пользователей.
        /// </summary>
        [HttpPost("jsonSerializeUsers")]
        public void SerializeJsonUsers()
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<User>));
            using (FileStream fileStream = new FileStream("Users.json", FileMode.Create))
            {
                jsonSerializer.WriteObject(fileStream, SortedUsers);
            }
        }

        /// <summary>
        /// Json сериализация списка писем.
        /// </summary>
        [HttpPost("jsonSerializeMails")]
        public void SerializeJsonMails()
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Mail>));
            using (FileStream fileStream = new FileStream("Mails.json", FileMode.Create))
            {
                jsonSerializer.WriteObject(fileStream, _mails);
            }
        }

        /// <summary>
        /// Json десериализация списка пользователей.
        /// </summary>
        [HttpPost("jsonDeserializeUsers")]
        public void DeserializeJsonUser()
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<User>));
            using (FileStream fileStream = new FileStream("Users.json", FileMode.Open))
            {
                _users = jsonSerializer.ReadObject(fileStream) as List<User>;
            }
        }

        /// <summary>
        /// Json десериализация списка писем.
        /// </summary>
        [HttpPost("jsonDeserializeMails")]
        public void DeserializeJsonMails()
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Mail>));
            using (FileStream fileStream = new FileStream("Mails.json", FileMode.Open))
            {
                _mails = jsonSerializer.ReadObject(fileStream) as List<Mail>;
            }
        }

        /// <summary>
        /// Получение отсортированного списка пользователей.
        /// </summary>
        /// <returns>Отсортированный список пользователей.</returns>
        [HttpGet]
        public IEnumerable<User> Get() => SortedUsers;

        /// <summary>
        /// Получение данных о пользователе по параметру email.
        /// </summary>
        /// <param name="email">Id пользователя.</param>
        /// <returns>Данные о пользователе.</returns>
        [HttpGet("{email}")]
        public IActionResult Get(string email)
        {
            var user = _users.SingleOrDefault(p => email == p.Email);

            return user == null ? NotFound() : Ok(user);
        }

        /// <summary>
        /// Получение писем по адресам отправителя и получателя.
        /// </summary>
        /// <param name="senderId">Отправитель.</param>
        /// <param name="receiverId">Получатель.</param>
        /// <returns>Данные о письмах.</returns>
        [HttpGet("{senderId}/{receiverId}")]
        public IActionResult GetMessagesBySenderAndReceiver(string senderId, string receiverId)
        { 
            var mails = _mails.FindAll(p => p.SenderId == senderId && p.ReceiverId == receiverId);

            return mails.Count != 0 ? Ok(mails) : NotFound();
        }

        /// <summary>
        /// Получение писем по адресу отправителя.
        /// </summary>
        /// <param name="email">Адрес отправителя.</param>
        /// <returns>Данные о письмах.</returns>
        [HttpGet("GetMessagesBySender/{email}")]
        public IActionResult GetMessagesBySender(string email)
        {
            var mails = _mails.FindAll(p => p.SenderId == email);

            return mails.Count != 0 ? Ok(mails) : Ok(new {Message = "0 messages was founded!"});
        }

        /// <summary>
        /// Получение писем по адресу получателя.
        /// </summary>
        /// <param name="email">Адрес получателя.</param>
        /// <returns>Данные о письмах.</returns>
        [HttpGet("GetMessagesByReceiver/{email}")]
        public IActionResult GetMessagesByReceiver(string email)
        {
            var mails = _mails.FindAll(p => p.ReceiverId == email);

            return mails.Count != 0 ? Ok(mails) : Ok(new {Message = "0 messages were founded!"});
        }
        
        /// <summary>
        /// Добавление новых пользователей.
        /// </summary>
        /// <param name="username">Имя пользователя.</param>
        /// <param name="email">Email адрес пользователя.</param>
        [HttpPost("add-user/{username}/{email}")]
        public void AddUser(string username, string email)
            => _users.Add(new User {UserName = username, Email = email});

        /// <summary>
        /// Отправка писем
        /// </summary>
        /// <param name="senderId">Отправитель</param>
        /// <param name="receiverId">Получатель</param>
        /// <param name="subject">Тема письма</param>
        /// <param name="message">Содержание письма</param>
        /// <returns>Код 200</returns>
        [HttpPost("add-message/{senderId}/{receiverId}")]
        public IActionResult AddMail(string senderId, string receiverId, string subject, string message)
        {
            var checkReceiver = _users.Find(x => x.Email == receiverId);
            var checkSender = _users.Find(x => x.Email == senderId);

            if (checkSender == null || checkReceiver == null) return NotFound();
            
            _mails.Add(new Mail
                {SenderId = senderId, ReceiverId = receiverId, Subject = subject, Message = message});

            return Ok();
        }

        /// <summary>
        /// Получение списка пользователей с Limit и Offset.
        /// </summary>
        /// <param name="limit">Количество пользователей, которое необходимо вернуть.</param>
        /// <param name="offset">Порядковый номер пользователя начиная
        /// с которого нужно начать получать информацию.</param>
        /// <returns>Отсортированный список пользователей начиная с отступа и до предела.</returns>
        [HttpGet("offset-users-get")]
        public IActionResult GetOffsetUsers(int limit, int offset)
        {
            limit = limit > _users.Count + 1 ? _users.Count + 1 : limit;
            offset = offset > _users.Count ? _users.Count : offset;

            if (limit < 1 || offset < 0) return BadRequest();

            return Ok(SortedUsers.Skip(offset).Take(limit));
        }
    }
}