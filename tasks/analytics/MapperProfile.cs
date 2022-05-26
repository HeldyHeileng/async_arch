using AutoMapper;
using analytics.Models;

namespace analytics;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

        CreateMap<Account, proto.AccountBalanceChangedV1>();
        CreateMap<proto.AccountBalanceChangedV1, Account>();

        CreateMap<Transaction, proto.TransactionAppliedV1>();
        CreateMap<proto.TransactionAppliedV1, Transaction>()
            .ForMember(x => x.CreatedAt, o => o.MapFrom(y => y.CreatedAt.Date));


        CreateMap<DateTime, proto.Tools.DateTimeWrapper>()
            .ForMember(x => x.Date, o => o.MapFrom(y => y));
    }
}