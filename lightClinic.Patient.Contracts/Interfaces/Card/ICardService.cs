using lightClinic.Patient.Contracts.DTOs.Card;
using lightClinic.Patient.Shared.Interfaces;

namespace lightClinic.Patient.Contracts.Interfaces.Card;

public interface ICardService
{
    Task<IResult<CardDto>> GetAsync(long id, CancellationToken cancellationToken);
    Task<IResult<List<CardDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<IResult<CardDto>> CreateAsync(CreateCardDto cardDto, CancellationToken cancellationToken);
    Task<IResult<CardDto>> UpdateAsync(UpdateCardDto cardDto, CancellationToken cancellationToken);
    Task<IResult<bool>> DeleteAsync(long id, CancellationToken cancellationToken);
    Task<IResult<bool>> ExistsAsync(long id, CancellationToken cancellationToken);
}