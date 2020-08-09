namespace AgonylMonsterEditor
{
    public class A3NPC
    {
        public string Name { get; set; }

        public ushort Id { get; set; }

        public ushort RespawnRate { get; set; }

        public byte Defense { get; set; }

        public byte AdditionalDefense { get; set; }

        public A3NPCAttack[] Attacks = new A3NPCAttack[3];

        public byte Level { get; set; }

        public uint HP { get; set; }

        public string Attack { get; set; }

        public string File { get; set; }

        public ushort AttackSpeedLow { get; set; }

        public ushort AttackSpeedHigh { get; set; }

        public ushort MovementSpeed { get; set; }

        public ushort PlayerExp { get; set; }

        public ushort BlueAttackDefense { get; set; }

        public ushort RedAttackDefense { get; set; }

        public ushort GreyAttackDefense { get; set; }

        public ushort MercenaryExp { get; set; }
    }
}
