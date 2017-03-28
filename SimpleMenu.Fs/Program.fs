open System
open NEventStore
open NEventStore.Persistence.Sql.SqlDialects

let wireUpEventStore =
    Wireup.Init()
        .LogToOutputWindow()
        .UsingSqlPersistence("EventStore")
        .WithDialect(new MsSqlDialect())
        .EnlistInAmbientTransaction()
        .InitializeStorageEngine()
        .UsingBinarySerialization()
        .Compress()
        .Build

[<EntryPoint>]
let main argv = 
    use store = wireUpEventStore()   
    let id = new Guid("247669b7-7810-4bc8-ad6d-353268067b57")
    let saver = EventStore.save store (printfn "Publishing event: %A")

    let p1 = Product.create id "Iceed Tee" "DRiinks"
    let savedP1 = Product.save saver p1        
    Console.ReadKey() |> ignore

    let p2 = savedP1 |> Product.updateName "Iced Tea"
    let p3 = p2 |> Product.updateCategory "Drinks"
    
    let saved = Product.save saver p3
    Console.ReadKey() |> ignore
    
    let product = EventStore.get store Product.factory id
    printfn "Product: %A" product

    Console.ReadKey() |> ignore
    0
