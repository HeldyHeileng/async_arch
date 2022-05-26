using AutoMapper;
using accounting.Models;

namespace accounting;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

        CreateMap<TrackerTask, proto.TaskAddedV1>();
        CreateMap<proto.TaskAddedV1, TrackerTask>()
            .ForMember(x => x.CreatedAt, o => o.MapFrom(y => y.CreatedAt.Date));
        CreateMap<TrackerTask, proto.TaskAddedV2>();
        CreateMap<proto.TaskAddedV2, TrackerTask>()
            .ForMember(x => x.CreatedAt, o => o.MapFrom(y => y.CreatedAt.Date));


        CreateMap<TrackerTask, proto.TaskCreatedV1>();
        CreateMap<proto.TaskCreatedV1, TrackerTask>()
            .ForMember(x => x.CreatedAt, o => o.MapFrom(y => y.CreatedAt.Date));
        CreateMap<TrackerTask, proto.TaskCreatedV2>();
        CreateMap<proto.TaskCreatedV2, TrackerTask>()
            .ForMember(x => x.CreatedAt, o => o.MapFrom(y => y.CreatedAt.Date));


        CreateMap<TrackerTask, proto.TaskCompletedV1>();
        CreateMap<proto.TaskCompletedV1, TrackerTask>()
            .ForMember(x => x.CreatedAt, o => o.MapFrom(y => y.CreatedAt.Date));
        CreateMap<TrackerTask, proto.TaskShuffledV1>();
        CreateMap<proto.TaskShuffledV1, TrackerTask>()
            .ForMember(x => x.CreatedAt, o => o.MapFrom(y => y.CreatedAt.Date));


        CreateMap<Account, proto.AccountBalanceChangedV1>();
        CreateMap<proto.AccountBalanceChangedV1, Account>();

        CreateMap<Transaction, proto.TransactionAppliedV1>();
        CreateMap<proto.TransactionAppliedV1, Transaction>()
            .ForMember(x => x.CreatedAt, o => o.MapFrom(y => y.CreatedAt.Date));


        CreateMap<DateTime, proto.Tools.DateTimeWrapper>()
            .ForMember(x => x.Date, o => o.MapFrom(y => y));

    }
}