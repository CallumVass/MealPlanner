module Extensions

type Deferred<'t> =
    | HasNotStartedYet
    | InProgress
    | Resolved of 't
