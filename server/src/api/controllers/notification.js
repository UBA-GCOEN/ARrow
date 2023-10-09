import notificationModel from "../models/notificationModel.js"
import userModel from "../models/userModel.js"


/**
 * Route: POST /notification/send
 * Desc: send notification
 */
export const sendNotification = async (req, res) => {
    const { 
        title, 
        message, 
        receiverRole, 
        receiverBranch, 
        receiverYear
    } = req.body

    if(typeof title != 'string'){
        res.send("invalid title")
        return 
    }

    if(typeof message != 'string'){
        res.send("invalid message")
        return 
    }

    if( !Array.isArray(receiverRole)){
        res.send("invalid receiverRole")
        return 
    }

    // if( !Array.isArray(receiverBranch)){
    //     res.send("invalid receiverBranch")
    //     return 
    // }

    // if( !Array.isArray(receiverYear)){
    //     res.send("invalid receiverYear")
    //     return 
    // }


    const email = req.email

    const oldUser = await userModel.findOne({email})
    

    const result = await notificationModel.create({
        title,
        message,
        senderEmail: email,
        senderName: oldUser.name,
        senderRole: oldUser.role,
        receiverRole,
        receiverBranch,
        receiverYear
    })

    if(result){
        res.status(200).json({
            msg: "notification sent succesffully",
            id: result.id

        })
    }

}



/**
 * Route GET /notification/get
 * Desc: get the notification sent  
 *       to you
 */
export const getNotification = async (req, res) => {

    const email = req.email

    const oldUser = await userModel.findOne({email})

    const role = oldUser.role

    const notifications = await notificationModel.find({
        receiverRoles: { $in: [role] }
    })

    if(notifications){
        res.json({
            msg: "notification received successfully",
            notifn: notifications
        })
    }
}






/**
 * Route: DELETE /notification/delete
 * Desc: delete the notification using id
 */
export const deleteNotification = async (req, res) => {
    const id = req.body._id

    try{
        const result = await notificationModel.findByIdAndDelete(id)

        if(result){
            res.status(200).json({
                msg: "notification deleted successfully"
            })
        }
        else{
            res.status(400).json({
                msg: "failed deleting notification"
            })
        }
    }
    catch(err){
        res.status(400).send(err)
    }
}