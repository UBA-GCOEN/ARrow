import bcrypt from 'bcrypt'
import userModel from "../models/userModel.js"




/**
 * Route: /changePassword
 * Desc: change the password
 */
export const changePassword = async (req, res) => {
    const role = req.role

    const email = req.session.user.user.email

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


        //geting userschema based on role
        if(role == 'admin'){
            oldUser = await userAdminModel.findOne({email})
        }
        else if(role == 'student'){
            oldUser = await userStudentModel.findOne({email})
        }
        else if(role == 'faculty'){
            oldUser = await userFacultyModel.findOne({email})
        }
        else if(role == 'staff'){
            oldUser = await userStaffModel.findOne({email})
        }
        else if(role == 'visitor'){
            oldUser = await userVisitorModel.findOne({email})
        }
        else{
            res.send("invalid role")
            return
        }


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