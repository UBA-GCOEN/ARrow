import eventModel from '../models/eventModel.js'
import userModel from '../models/userModel.js'

/**
 * Route: /event/createEvent
 * Desc: create event 
 */
export const createEvent = async (req, res) => {
    let email = req.email

    const { 
        title,
        description,
        eventCoordinator, 
        time,
        venue,
        guest,  
    } = req.body



    /**
     * adding validation to prevent sql injection
     */
    if(typeof title !== 'string' ){
        res.send("inalid title")
        return 
    }

    if(typeof description !== 'string' ){
        res.send("invalid description")
        return
    }

    if(typeof eventCoordinator !== 'string' ){
        res.send("invalid eventCoordinatot")
        return
    }

    if(typeof time !== 'string' ){
        res.send("invalid time")
        return
    }

    if(typeof venue !== 'string' ){
        res.send("invalid venue")
        return
    }

    if(typeof guest !== 'string' ){
        res.send("invalid guest")
        return
    }

    if(typeof email !== 'string' ){
        res.send("invalid email")
        return
    }

    //detrmine current logged in role 
    const olduser = await userModel.findOne({email})
    const role = olduser.role

    
    // Event creation in database
     
    if(role == 'Admin' || role == 'Faculty'){

       let organizerRole = role
       let organizerEmail = olduser.email
       let organizerName = olduser.name

       console.log(organizerRole, organizerEmail, organizerName);

        try{
            const result = await eventModel.create({
                title,
                description,
                organizerRole,
                organizerEmail,
                organizerName,
                eventCoordinator,
                time,
                venue,
                guest,
            })
    
            if(result){
                res.json({
                    msg: "event created successfully",
                    _id: result._id,
                    event: result
                })
            }
            else{
                res.send("event failed")
            }
        }
        catch(err){
            console.log(err)
        }

    }
    else {
        res.send("only faculties and admins have permission to create events")
        return
    }

}







/**
 * Route: /event/updateEvent
 * Desc: update the event information or status
 */
export const updateEvent = async (req, res) => {
    let email = req.email

    const { 
        _id,
        title,
        description,
        eventCoordinator, 
        time,
        venue,
        guest,  
    } = req.body



    /**
     * adding validation to prevent sql injection
     */
        if(typeof _id !== "string" ){
            res.status(400).json({ status: "invalid _id" });
            return;
        } 

        if(typeof title !== "string" ){
            res.status(400).json({ status: "invalid title" });
            return;
        }
    
        if(typeof description !== "string"){
            res.status(400).json({ status: "invalid description" });
            return;
        }
    
        if(typeof eventCoordinator !== "string" ){
            res.status(400).json({ status: "invalid eventCoordinator" });
            return;
        }
    
        if(typeof time !== "string" ){
            res.status(400).json({ status: "invalid time" });
            return;
        }
    
        if(typeof venue !== "string" ){
            res.status(400).json({ status: "invalid venue" });
            return;
        }
    
        if(typeof guest !== "string" ){
            res.status(400).json({ status: "invalid guest" });
            return;
        }



    //detrmine current logged in role 
    const olduser = await userModel.findOne({email})
    const role = olduser.role


    // Event updation in database
    if(role == 'admin' || role == 'faculty'){

        let organizerRole = role
        let organizerEmail = req.session.user.user.email
        let organizerName = req.session.user.user.name

        try{

            const event = await eventModel.findOne({_id})

            if(!event){
                res.send("event does not exist")
                return
            }

            const result = await event.updateOne({
                title,
                description,
                organizerRole,
                organizerEmail,
                organizerName,
                eventCoordinator,
                time,
                venue,
                guest,
            })
    
            if(result){
                res.send("event udated successfully")
            }
        }
        catch(err){
            console.log(err)
        }

    }
    else {
        res.send("only faculties and admins have permission to update events")
        return
    }
}







/**
 * Route: /event/deleteEvent
 * Desc: delete the event
 */
export const deleteEvent = async (req, res) =>{
    let email = req.email

    const { 
        _id 
    } = req.body


    //sql attack prevention
    if(typeof _id !== 'string' ){
        res.send("inalid title")
        return 
    }



    //detrmine current logged in role 
    const olduser = await userModel.findOne({email})
    const role = olduser.role    


    
    if(role == 'admin' || role == 'faculty'){

        try{
            const event = await eventModel.findOne({_id})

            if(event){
            const result = await event.deleteOne({_id})
                if(result){
                    res.send("event deleted successfully")
                }
                else{
                    res.send("error deleting event")
                }
            }
            else{
                res.send("event does not exist")
            }
        

        }
        catch(err){
            res.send(err)
            return
        }

    }
    else {
        res.send("only faculties and admins have permission to delete events")
        return
    }  
}



/**
 * Route: /event/getSpecificEvent
 * Desc: get event details
 */
export const getSpecificEvent = async (req, res) => {

    const _id = req.body._id

    //sql attack prevention
    if(typeof _id !== 'string' ){
        res.send("inalid title")
        return 
    }

    try{
        const result = await eventModel.findOne({_id})

        if(result){
            res.send(result)
        }
        else{
            res.send("event does not exist")
        }
    }
    catch(err){
        console.log(err)
    }
  
}



/**
 * Route: /event/getAllEvents
 * Desc: get all events
 */
export const getAllEvents = async (req, res) =>{
    
    try{
        const result = await eventModel.find({})
        res.send(result)
    }
    catch(err){
        res.send(err)
        return
    }
}