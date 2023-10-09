import bcrypt from 'bcrypt'
import userModel from "../models/userModel.js"




/**
 * Route: /changePassword
 * Desc: change the password
 */
export const changePassword = async (req, res) => {

    const email = req.email

    const oldPassword = req.body.oldPassword
    const newPassword = req.body.newPassword
    const confirmNewPassword = req.body.confirmNewPassword

    var isPasswordCorrect = ''
    var oldUser = ''
    var newUser = ''

    
    //password confirmation
    if(newPassword !== confirmNewPassword){
        res.send("new Password and confirm new password doesnt match")
        return
    }
    try {


        oldUser = await userModel.findOne({email})



        //hashing and updating new password
        const hashedPassword = await bcrypt.hash(newPassword, 12)
        isPasswordCorrect = await bcrypt.compare(oldPassword, oldUser.password)
        if(isPasswordCorrect){
            newUser = await oldUser.updateOne({
                password: hashedPassword
            })
        }
        else{
            res.send("incorrect old password")
            return
        }


    } catch (error) {
        console.log(error)
        return
    }

    

    if(newUser){
        res.send("password changed successfully")
    }


}