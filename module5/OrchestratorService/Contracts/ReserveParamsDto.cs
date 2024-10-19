namespace OrchestratorService.Contracts
{
    /// <summary>
    /// Параметры резервирования.
    /// </summary>
    public class ReserveParamsDto
    {
        /// <summary>
        /// Оружие для резервирования.
        /// </summary>
        public WeaponDto Weapon { get; set; }

        /// <summary>
        /// Очередь пользователей.
        /// </summary>
        public List<UserDto> UsersQueue { get; set; }

    }
}
