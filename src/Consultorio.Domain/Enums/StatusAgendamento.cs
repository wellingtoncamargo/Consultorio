namespace Consultorio.Domain.Enums 
{
    public enum StatusAgendamento
    {
        Agendado = 1,
        Confirmado = 2,
        Realizado = 3,
        Cancelado = 4,
        NaoCompareceu = 5
    }

    public enum TipoNotificacao
    {
        Email = 1,
        WhatsApp = 2,
        Ambos = 3
    }
}

