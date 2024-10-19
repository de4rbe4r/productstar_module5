namespace WeaponsService2.Contracts
{
    /// <summary>
    /// Оружие.
    /// </summary>
    public class WeaponDto
    {
        /// <summary>
        /// Id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Название.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Количество.
        /// </summary>
        public int Quantity { get; set; }
    }
}
