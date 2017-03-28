module Product

open System

type Event = 
    | Created of Guid * string * string * DateTime 
    | NameChanged of string 
    | CategoryChanged of string

type State = { id : Guid; name : string; category : string; created : DateTime }

type Product = { state : State; unsavedEvents : Event list }

let private zero = { id = Guid.Empty; name = ""; category = ""; created = DateTime.MinValue }

let private update state event =
    match event with
    | Created (id, name, category, created) -> { id = id; name = name; category = category; created = DateTime.UtcNow }
    | NameChanged (newName) -> { state with name = newName }
    | CategoryChanged (newCategory) -> { state with category = newCategory }

let factory events =
    let state = events |> Seq.fold update zero
    { state = state; unsavedEvents = [] }

let save saver product =
    let { state = state; unsavedEvents = events } = product
    saver state.id (List.rev events)
    printfn "Product saved."
    { product with unsavedEvents = [] }

let create id name category =
    // check invariants here
    let event = Created (id, name, category, DateTime.UtcNow)        
    { state = update zero event; unsavedEvents = [ event ] }

let updateName newName product =
    let { state = oldState; unsavedEvents = events } = product
    if String.length newName < 3 then failwith "Invalid product name."
             
    let event = NameChanged newName
    { state = update oldState event; unsavedEvents = event::events }

let updateCategory newCategory product =
    let { state = oldState; unsavedEvents = events } = product
    // check invariants here
    let event = CategoryChanged newCategory
    { state = update oldState event; unsavedEvents = event::events }
 