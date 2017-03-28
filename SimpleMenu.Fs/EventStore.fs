module EventStore

open System
open NEventStore

let private persist (eventStore : IStoreEvents) (id:Guid) events =                    
    use stream = eventStore.OpenStream id
    events |> List.iter (fun ev -> stream.Add (new EventMessage (Body = ev)))
    stream.CommitChanges (Guid.NewGuid())
    ()

let save eventStore publish id events =
    persist eventStore id events
    events |> List.iter publish
    ()

let find (eventStore : IStoreEvents) factory (id:Guid) =
    use stream = eventStore.OpenStream id
    match stream.StreamRevision with
    | 0 -> None
    | x -> stream.CommittedEvents
        |> Seq.map (fun msg -> msg.Body)
        |> Seq.cast<'e>
        |> factory
        |> Some

let get (eventStore : IStoreEvents) factory (id:Guid) =
    let result = find eventStore factory id
    match result with
    | Some ar -> ar
    | None -> failwithf "Aggregate not found with id: %A" id
