namespace SP2016.Repository.Enums
{
    public enum MembershipFilterType
    {
        /// <summary>
        /// Возвращает задачи, назначенные группам, в которые входит текущий пользователь
        /// </summary>
        CurrentUserGroups,
        /// <summary>
        /// Возвращает задачи, назначенные группам сайта
        /// </summary>
        Groups,
        /// <summary>
        /// Возвращает задачи, назначенные пользователям, но не группам
        /// </summary>
        AllUsers,
        /// <summary>
        /// Возвращает задачи, назначенные пользователям, которым доступ к сайту был предоставлен напрямую
        /// </summary>
        Users,
        /// <summary>
        /// Возвращает задачи, назначенные конкретной группе
        /// </summary>
        SPGroup
    }
}
