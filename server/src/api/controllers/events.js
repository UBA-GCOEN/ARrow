import express from 'express'
import eventModel from '../models/eventModel.js'


/**
 * Route: /event/createEvent
 * Desc: create event 
 */
export const createEvent = async (req, res) => {
    let role = req.role

    const { 
        title,
        description,
        eventCoordinator, 
        time,
        venue,
        guest,  
    } = req.body

    if(role == 'admin' || role == 'faculty'){

       let organizerRole = role
       let organizerEmail = req.session.user.user.email
       let organizerName = req.session.user.user.name

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
    let role = req.role

    const { 
        _id,
        title,
        description,
        eventCoordinator, 
        time,
        venue,
        guest,  
    } = req.body

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
    let role = req.role

    const { 
        _id 
    } = req.body

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