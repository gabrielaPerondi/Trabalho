using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrabalhoElvis2.Models
{
    public class Condomino
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Telefone inválido.")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Selecione o tipo do condômino.")]
        public string Tipo { get; set; } = "Proprietário"; // Proprietário ou Locatário

        [DataType(DataType.Date)]
        public DateTime? InicioLocacao { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FimLocacao { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? ValorAluguel { get; set; }

        public string? Observacoes { get; set; }

        // 🔗 Relação 1:N (um condômino -> vários imóveis)
        public ICollection<Imovel>? Imoveis { get; set; }
    }
}
