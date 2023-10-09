import userModel from "../models/userModel.js"
import bcrypt from 'bcrypt'




/**
 * Route: GET /getDeletePage
 * Desc: get the details of 
 *       current user before 
 *       deleting the user
 */
export const getDeletePage = async (req, res) => {

    var email = req.email
    var name = req.session.user.user.name

    res.status(200).json({
        name,
        email,
    })
}





/**
 * Route: DELETE /deleteUser
 * Desc: Delete the current 
 *       logged in user
 */
export const deleteUser = async (req, res) => {
 
    var { name, password } = req.body

    var email = req.email

    const emailDomains = [
        "@gmail.com",
        "@yahoo.com",
        "@hotmail.com",
        "@aol.com",
        "@outlook.com",
        "@gcoen.ac.in",
        ];

 
 
        //check name length
        if (name.length < 2) {
          return res
            .status(404)
            .json({ message: "Name must be atleast 2 characters long." });
        }
 
        // check email format
        if (!emailDomains.some((v) => email.indexOf(v) >= 0)) {
          return res.status(404).json({
         message: "Please enter a valid email address",
       })};




       /**
        * checking field types
        * to avoid sql attacks
        */
        if (typeof name !== "string") {
            res.status(400).json({ status: "error"+name+ typeof name });
            return;
        }

        if (typeof email !== "string") {
            res.status(400).json({ status: "error"+email+typeof email });
            return;
        }
      

        // delete user       
        var oldUser = await userModel.findOne({email})
        var isPasswordCorrect = bcrypt.compare(password, oldUser.password)
        
        if(isPasswordCorrect) {
            oldUser = await userModel.deleteOne({email})
            res.send("User deleted successffuly")
        }
        else{
            res.send("Incorrect Password")
        }
    
}